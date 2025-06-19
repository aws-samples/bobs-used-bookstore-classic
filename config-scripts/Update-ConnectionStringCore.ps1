$myPath = Split-Path $MyInvocation.MyCommand.Path -Parent

function Get-ConnectionString {
    $sqlUsername = ((Get-SECSecretValue -SecretId "SQLServerRDSSecret").SecretString | ConvertFrom-Json).username
    $sqlPassword = ((Get-SECSecretValue -SecretId "SQLServerRDSSecret").SecretString | ConvertFrom-Json).password

    $endpointAddress = Get-RDSDBInstance | Select-Object -ExpandProperty Endpoint | select Address
    [string] $SQLDatabaseEndpoint = $endpointAddress.Address

    [string] $SQLDatabaseEndpointTrimmed = $SQLDatabaseEndpoint.Replace(':1433','')
    [string] $retConnectionString = "Server=$SQLDatabaseEndpointTrimmed;Database=BookStoreClassic;User Id=$sqlUsername;Password=$sqlPassword;Trusted_Connection=false"
    return $retConnectionString
}

function Update-ConnectionString-AppSettings {
    param (
        [string] $connectionStringParam,
        [string] $appSettingsPathParam
    )
    
    $appSettingsPathParam = Join-Path $myPath $appSettingsPathParam
    
    $jsonContent = Get-Content -Path $appSettingsPathParam -Raw | ConvertFrom-Json
    $jsonContent.ConnectionStrings.BookstoreDatabaseConnection = $connectionStringParam
    $jsonString = $jsonContent | ConvertTo-Json -Depth 10
    Set-Content -Path $appSettingsPathParam -Value $jsonString -Encoding UTF8
}

$connectionString = Get-ConnectionString
$appSettings1 = "..\app\Bookstore.Web\appsettings.json"

Update-ConnectionString-AppSettings -connectionStringParam $connectionString -appSettingsPathParam $appSettings1
