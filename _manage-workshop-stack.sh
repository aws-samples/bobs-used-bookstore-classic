#!/bin/bash -x
# Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
# SPDX-License-Identifier: MIT-0

main() {
    STACK_OPERATION=$1

    if [[ "$STACK_OPERATION" == "create" || "$STACK_OPERATION" == "update" ]]; then
        echo "=== Enabling AWS IAM Identity Center ==="

        # Check if AWS CLI is configured
        if ! aws sts get-caller-identity &>/dev/null; then
            echo "Error: AWS CLI is not configured or you don't have proper permissions"
            exit 1
        fi
        echo "✓ AWS CLI is configured"

        if aws sso-admin list-instances --query 'Instances[0].InstanceArn' --output text 2>/dev/null | grep -q "arn:aws:sso"; then
            echo "✓ IAM Identity Center already enabled, skipping creation"
        elif aws sso-admin create-instance --name "Default" 2>/dev/null; then
            echo "✓ IAM Identity Center enabled successfully via Organizations"
            sleep 10  # Wait for service to initialize
        else
            echo "Failed to enable via Organizations API"
            exit 1
        fi

        # Wait for Identity Center to become available
        echo "Waiting for Identity Center to initialize..."
        for i in {1..12}; do
            if aws sso-admin list-instances --query 'Instances[0].InstanceArn' --output text 2>/dev/null | grep -q "arn:aws:sso"; then
                echo "✓ IAM Identity Center is now enabled"
                INSTANCE_ARN=$(aws sso-admin list-instances --query 'Instances[0].InstanceArn' --output text)
                IDENTITY_STORE_ID=$(aws sso-admin list-instances --query 'Instances[0].IdentityStoreId' --output text)
                echo "Instance ARN: $INSTANCE_ARN"
                echo "Identity Store ID: $IDENTITY_STORE_ID"
                break
            fi
            echo "Waiting... (attempt $i/12)"
            sleep 10
        done

        # Check if enablement failed
        if ! aws sso-admin list-instances --query 'Instances[0].InstanceArn' --output text 2>/dev/null | grep -q "arn:aws:sso"; then
            echo "❌ Automatic enablement failed."
            exit 1
        fi

        # AWS CLI script to create a user in AWS IAM Identity Center
        # Configuration variables
        IDENTITY_STORE_ID="d-xxxxxxxxxx"
        USERNAME="workshop-user"
        GIVEN_NAME="workshop"
        FAMILY_NAME="user"
        EMAIL="workshop.user@example.com"
        DISPLAY_NAME="Workshop User"

        echo "=== AWS Identity Center User Creation Script ==="

        echo "Getting Identity Store ID..."
        IDENTITY_STORE_ID=$(aws sso-admin list-instances --query 'Instances[0].IdentityStoreId' --output text | head -n 1)
        echo "Identity Store ID: $IDENTITY_STORE_ID"

        # Validate required variables
        if [ -z "$IDENTITY_STORE_ID" ]; then
            echo "Error: Please set the IDENTITY_STORE_ID variable"
            exit 1
        fi

        if [ -z "$USERNAME" ] || [ -z "$EMAIL" ]; then
            echo "Error: USERNAME and EMAIL must be set"
            exit 1
        fi

        # Create the user (skip if already exists)
        echo "Creating user: $USERNAME"
        EXISTING_USER_ID=$(aws identitystore list-users \
            --identity-store-id "$IDENTITY_STORE_ID" \
            --filters AttributePath="UserName",AttributeValue="$USERNAME" \
            --query 'Users[0].UserId' \
            --output text 2>/dev/null)

        if [ -n "$EXISTING_USER_ID" ] && [ "$EXISTING_USER_ID" != "None" ]; then
            echo "✓ User $USERNAME already exists (ID: $EXISTING_USER_ID), skipping creation"
            USER_ID="$EXISTING_USER_ID"
        else
            USER_ID=$(aws identitystore create-user \
                --identity-store-id "$IDENTITY_STORE_ID" \
                --user-name "$USERNAME" \
                --name GivenName="$GIVEN_NAME",FamilyName="$FAMILY_NAME" \
                --display-name "$DISPLAY_NAME" \
                --emails Value="$EMAIL",Type="work",Primary=true \
                --query 'UserId' \
                --output text)

            if [ $? -eq 0 ]; then
                echo "User created successfully!"
                echo "User ID: $USER_ID"
                echo "Username: $USERNAME"
                echo "Email: $EMAIL"
            else
                echo "Failed to create user"
                exit 1
            fi
        fi

        # Verify creation
        echo "Verifying user creation..."
        aws identitystore describe-user \
            --identity-store-id "$IDENTITY_STORE_ID" \
            --user-id "$USER_ID" \
            --output table

        echo "=== Script completed ==="

    elif [ "$STACK_OPERATION" == "delete" ]; then
        echo "Done cdk destroy!"
    else
        echo "Invalid stack operation!"
        exit 1
    fi
}

STACK_OPERATION=$(echo "$1" | tr '[:upper:]' '[:lower:]')
main "$STACK_OPERATION"
