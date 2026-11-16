/*
  Stored procedure: dbo.usp_PlaceOrderFromCart
  Database:         BookStoreClassic (SQL Server)

  Purpose
  -------
  Converts the items a customer marked "WantToBuy" in a shopping cart into an
  order, as one atomic unit of work:
    1. Validates the customer, the delivery address and the cart contents.
    2. Locks the affected Book rows and verifies there is enough stock.
    3. Inserts the [Order] header and its [OrderItem] rows.
    4. Decrements Book.Quantity.
    5. Removes the purchased lines from the cart.
    6. Returns the new order id and order total.

  Run after bobs-used-bookstore-classic-db.sql.

  Constructs that are useful for demonstrating SQL Server -> Aurora PostgreSQL
  conversion (PL/pgSQL):
    - Bracketed identifiers and the reserved word [Order] used as a table name
    - OUTPUT parameters, default parameter values
    - SET NOCOUNT / SET XACT_ABORT
    - BEGIN TRY / BEGIN CATCH, THROW, RAISERROR, ERROR_MESSAGE()
    - BEGIN TRANSACTION / COMMIT / ROLLBACK, @@TRANCOUNT, XACT_STATE()
    - Table variable (@PurchaseLines)
    - Locking hints (UPDLOCK, ROWLOCK)
    - UPDATE ... FROM ... JOIN
    - IDENTITY columns and SCOPE_IDENTITY()
    - @@ROWCOUNT, GETDATE(), DATEADD(), ISNULL(), TOP, SUM()
*/
USE [BookStoreClassic]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[usp_PlaceOrderFromCart]
    @ShoppingCartId INT,
    @CustomerId     INT,
    @AddressId      INT,
    @DeliveryDate   DATETIME = NULL,       -- defaults to 7 days from now
    @CreatedBy      NVARCHAR(MAX) = N'usp_PlaceOrderFromCart',
    @OrderId        INT OUTPUT,
    @OrderTotal     DECIMAL(18, 2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Now DATETIME = GETDATE();
    DECLARE @LineCount INT;

    SET @OrderId = NULL;
    SET @OrderTotal = 0;

    -- Purchasable lines from the cart, with current book price and stock.
    DECLARE @PurchaseLines TABLE
    (
        BookId       INT            NOT NULL PRIMARY KEY,
        CartItemId   INT            NOT NULL,
        Quantity     INT            NOT NULL,
        UnitPrice    DECIMAL(18, 2) NOT NULL
    );

    BEGIN TRY
        ---------------------------------------------------------------------
        -- Validation
        ---------------------------------------------------------------------
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Customer] WHERE [Id] = @CustomerId)
            THROW 50001, 'Customer does not exist.', 1;

        IF NOT EXISTS (SELECT 1
                       FROM [dbo].[Address]
                       WHERE [Id] = @AddressId
                         AND [CustomerId] = @CustomerId
                         AND [IsActive] = 1)
            THROW 50002, 'Address does not exist, is inactive, or does not belong to the customer.', 1;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[ShoppingCart] WHERE [Id] = @ShoppingCartId)
            THROW 50003, 'Shopping cart does not exist.', 1;

        IF @DeliveryDate IS NULL
            SET @DeliveryDate = DATEADD(DAY, 7, @Now);

        IF @DeliveryDate < @Now
            THROW 50004, 'Delivery date cannot be in the past.', 1;

        ---------------------------------------------------------------------
        -- Collect the lines to purchase (a book appears once per cart line;
        -- aggregate defensively in case of duplicates).
        ---------------------------------------------------------------------
        INSERT INTO @PurchaseLines (BookId, CartItemId, Quantity, UnitPrice)
        SELECT sci.[BookId],
               MIN(sci.[Id]),
               SUM(sci.[Quantity]),
               b.[Price]
        FROM [dbo].[ShoppingCartItem] AS sci
        INNER JOIN [dbo].[Book] AS b
            ON b.[Id] = sci.[BookId]
        WHERE sci.[ShoppingCartId] = @ShoppingCartId
          AND sci.[WantToBuy] = 1
        GROUP BY sci.[BookId], b.[Price];

        SET @LineCount = @@ROWCOUNT;

        IF @LineCount = 0
            THROW 50005, 'Shopping cart has no items marked to buy.', 1;

        BEGIN TRANSACTION;

        ---------------------------------------------------------------------
        -- Lock the books being bought and verify stock.
        ---------------------------------------------------------------------
        IF EXISTS (SELECT 1
                   FROM @PurchaseLines AS pl
                   INNER JOIN [dbo].[Book] AS b WITH (UPDLOCK, ROWLOCK)
                       ON b.[Id] = pl.BookId
                   WHERE b.[Quantity] < pl.Quantity)
        BEGIN
            DECLARE @ShortBook NVARCHAR(MAX);

            SELECT TOP (1) @ShortBook = b.[Name]
            FROM @PurchaseLines AS pl
            INNER JOIN [dbo].[Book] AS b
                ON b.[Id] = pl.BookId
            WHERE b.[Quantity] < pl.Quantity
            ORDER BY b.[Id];

            DECLARE @Msg NVARCHAR(2048) =
                N'Insufficient stock for book: ' + ISNULL(@ShortBook, N'(unknown)');
            THROW 50006, @Msg, 1;
        END

        ---------------------------------------------------------------------
        -- Create the order (OrderStatus 1 = Ordered; see OrderStatus enum).
        ---------------------------------------------------------------------
        INSERT INTO [dbo].[Order]
            ([CustomerId], [AddressId], [DeliveryDate], [OrderStatus],
             [CreatedBy], [CreatedOn], [UpdatedOn])
        VALUES
            (@CustomerId, @AddressId, @DeliveryDate, 1,
             @CreatedBy, @Now, @Now);

        SET @OrderId = SCOPE_IDENTITY();

        INSERT INTO [dbo].[OrderItem]
            ([OrderId], [BookId], [Quantity], [CreatedBy], [CreatedOn], [UpdatedOn])
        SELECT @OrderId, pl.BookId, pl.Quantity, @CreatedBy, @Now, @Now
        FROM @PurchaseLines AS pl;

        ---------------------------------------------------------------------
        -- Decrement inventory.
        ---------------------------------------------------------------------
        UPDATE b
        SET b.[Quantity]  = b.[Quantity] - pl.Quantity,
            b.[UpdatedOn] = @Now
        FROM [dbo].[Book] AS b
        INNER JOIN @PurchaseLines AS pl
            ON pl.BookId = b.[Id];

        ---------------------------------------------------------------------
        -- Remove purchased lines from the cart.
        ---------------------------------------------------------------------
        DELETE sci
        FROM [dbo].[ShoppingCartItem] AS sci
        WHERE sci.[ShoppingCartId] = @ShoppingCartId
          AND sci.[WantToBuy] = 1;

        SELECT @OrderTotal = ISNULL(SUM(pl.Quantity * pl.UnitPrice), 0)
        FROM @PurchaseLines AS pl;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        SET @OrderId = NULL;
        SET @OrderTotal = 0;

        -- Re-raise the original error to the caller.
        THROW;
    END CATCH
END
GO

/*
  Example usage:

  DECLARE @NewOrderId INT, @Total DECIMAL(18, 2);

  EXEC [dbo].[usp_PlaceOrderFromCart]
       @ShoppingCartId = 1,
       @CustomerId     = 1,
       @AddressId      = 1,
       @DeliveryDate   = NULL,
       @OrderId        = @NewOrderId OUTPUT,
       @OrderTotal     = @Total OUTPUT;

  SELECT @NewOrderId AS OrderId, @Total AS OrderTotal;
*/
