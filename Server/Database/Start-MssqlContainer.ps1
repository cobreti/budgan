<#
.SYNOPSIS
    Starts a SQL Server (MSSQL) Docker container with port forwarding.

.PARAMETER Password
    Password for the login. Must satisfy SQL Server complexity rules
    (8+ chars, upper+lower+digit or symbol). Also used as the SA password internally
    when -Username is not "sa".

.PARAMETER Username
    SQL login to connect with. Defaults to "sa". If set to anything else, a new SQL
    login with that name is created (with sysadmin rights) after the container starts.

.PARAMETER Port
    Host port to forward to the container's 1433 port. Defaults to 1433.

.PARAMETER ContainerName
    Name for the Docker container. Defaults to "budgan-mssql".

.PARAMETER Image
    MSSQL Docker image to use. Defaults to "mcr.microsoft.com/mssql/server:2022-latest".

.PARAMETER DatabaseName
    Name of the database to create if it doesn't already exist. Defaults to "Budgan".

.PARAMETER VolumeName
    Name of the Docker named volume used to persist database files across container
    recreation. Created automatically if it doesn't already exist. Defaults to
    "budgan-mssql-data".

.EXAMPLE
    ./Start-MssqlContainer.ps1 -Password "YourStr0ng!Pass"

.EXAMPLE
    ./Start-MssqlContainer.ps1 -Password "YourStr0ng!Pass" -Username "budgan_user" -Port 14330 -ContainerName "budgan-mssql-dev"
#>

param(
    [string]$Password = "P@ssword",

    [string]$Username = "sa",

    [int]$Port = 1433,

    [string]$ContainerName = "budgan-mssql",

    [string]$Image = "mcr.microsoft.com/mssql/server:2022-latest",

    [string]$DatabaseName = "Budgan",

    [string]$VolumeName = "budgan-mssql-data"
)

$ErrorActionPreference = "Stop"

$existing = docker ps -a --filter "name=^/$ContainerName$" --format "{{.Names}}"
if ($existing -eq $ContainerName) {
    Write-Host "Container '$ContainerName' already exists. Starting it..."
    docker start $ContainerName
} else {
    Write-Host "Pulling image '$Image'..."
    docker pull $Image

    $existingVolume = docker volume ls --filter "name=^$VolumeName$" --format "{{.Name}}"
    if ($existingVolume -ne $VolumeName) {
        Write-Host "Creating volume '$VolumeName'..."
        docker volume create $VolumeName | Out-Null
    }

    Write-Host "Starting container '$ContainerName' on port $Port..."
    docker run -d `
        --name $ContainerName `
        -e "ACCEPT_EULA=Y" `
        -e "MSSQL_SA_PASSWORD=$Password" `
        -p "${Port}:1433" `
        -v "${VolumeName}:/var/opt/mssql" `
        $Image
}

Write-Host "Waiting for SQL Server to become ready..."
$ready = $false
for ($i = 0; $i -lt 30; $i++) {
    Start-Sleep -Seconds 2
    docker exec $ContainerName /opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P $Password -Q "SELECT 1" *> $null
    if ($LASTEXITCODE -eq 0) {
        $ready = $true
        break
    }
}

if (-not $ready) {
    throw "SQL Server did not become ready in time."
}

Write-Host "Creating database '$DatabaseName' if it doesn't exist..."
$createDbSql = "IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = '$DatabaseName') BEGIN CREATE DATABASE [$DatabaseName]; END"
docker exec $ContainerName /opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P $Password -Q $createDbSql

if ($Username -ne "sa") {
    Write-Host "Creating login '$Username'..."
    $createLoginSql = "IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = '$Username') BEGIN CREATE LOGIN [$Username] WITH PASSWORD = '$Password'; ALTER SERVER ROLE sysadmin ADD MEMBER [$Username]; END"
    docker exec $ContainerName /opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P $Password -Q $createLoginSql

    Write-Host "Container '$ContainerName' started. Connect on localhost,$Port with user '$Username', database '$DatabaseName'."
} else {
    Write-Host "Container '$ContainerName' started. Connect on localhost,$Port with user 'sa', database '$DatabaseName'."
}