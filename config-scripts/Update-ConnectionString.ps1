$myPath = Split-Path $MyInvocation.MyCommand.Path -Parent

function Get-ConnectionString {
    $sqlUsername = ((Get-SECSecretValue -SecretId "SQLServerRDSSecret").SecretString | ConvertFrom-Json).username
    $sqlPassword = ((Get-SECSecretValue -SecretId "SQLServerRDSSecret").SecretString | ConvertFrom-Json).password

    $endpointAddress = Get-RDSDBInstance | Select-Object -ExpandProperty Endpoint | select Address
    [string] $SQLDatabaseEndpoint = $endpointAddress.Address

    [string] $SQLDatabaseEndpointTrimmed = $SQLDatabaseEndpoint.Replace(':1433','')
    [string] $retConnectionString = "Server=$SQLDatabaseEndpointTrimmed;Database=BookStoreClassic;User Id=$sqlUsername;Password=$sqlPassword;"
    return $retConnectionString
}

function Update-ConnectionString-WebConfig {
    param (
        [string] $connectionStringParam,
        [string] $webConfigPathParam
    )
    
    $webConfigPathParam = Join-Path $myPath $webConfigPathParam
    $webConfigXml = [xml](Get-Content -Path $webConfigPathParam)

    $addElement = $webConfigXml.configuration.appSettings.add | Where-Object { $_.key -eq "ConnectionStrings/BookstoreDatabaseConnection" }
    $addElement.value = $connectionStringParam

    $webConfigXml.Save($webConfigPathParam)
}

$connectionString = Get-ConnectionString
$webConfig1 = "..\app\Bookstore.Web\Web.config"

Update-ConnectionString-WebConfig -connectionStringParam $connectionString -webConfigPathParam $webConfig1
