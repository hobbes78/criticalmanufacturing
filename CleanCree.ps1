# This script can be run upon the start of each user story
# It cleans the whole environment, rebuilds everything in a few minutes
# Launches the message bus, host and web server

################################################################################
##### Configurable Parameters

$CreeRepo = "C:\GitReps\CREE"
$DevBranch = "development"
$SolutionFile = "$CreeRepo\Business\Business.sln"
$DbServer = "127.0.0.1"
$DbName = "SUNY"
$DbUser = "CMFUser"
$DbPassword = "CMFUser"

################################################################################
##### Utility functions

# https://mcpmag.com/articles/2018/12/10/test-sql-connection-with-powershell.aspx
function Test-SqlConnection {
    param(
        [Parameter(Mandatory)]
        [string]$ServerName,

        [Parameter(Mandatory)]
        [string]$DatabaseName,

        [Parameter(Mandatory)]
        [pscredential]$Credential
    )

    $ErrorActionPreference = 'Stop'

    try {
        $userName = $Credential.UserName
        $password = $Credential.GetNetworkCredential().Password
        $connectionString = 'Data Source={0};database={1};User ID={2};Password={3}' -f $ServerName,$DatabaseName,$userName,$password
        $sqlConnection = New-Object System.Data.SqlClient.SqlConnection $ConnectionString
        $sqlConnection.Open()
        ## This will run if the Open() method does not throw an exception
        $true
    } catch {
        $false
    } finally {
        ## Close the connection when we're done
        $sqlConnection.Close()
    }
}

################################################################################
##### Script body

Write-Output ">>> Finding gulp filepath..."
$GulpLocation = "${env:APPDATA}\npm\gulp.cmd"
if (-not(Test-Path -Path "$GulpLocation" -PathType Leaf)) {
    $GulpLocation = "${env:ProgramFiles}\nodejs\gulp.cmd"
    if (-not(Test-Path -Path "$GulpLocation" -PathType Leaf)) {
        Write-Output "Error: gulp wasn't found anywhere"
        exit
    }
}

Set-Location $CreeRepo

Write-Output ">>> Undoing all changes..."
git reset --hard

Write-Output ">>> Removing all files and directories not under source-control..."
git clean -fdx

Write-Output ">>> Switching to branch $DevBranch..."
git checkout $DevBranch

Write-Output ">>> Pulling latest changes..."
git pull

Set-Location "$CreeRepo\Tools"

Write-Output ">>> Obtaining local environment..."
& ".\Local_GetLocalEnvironment.ps1"

Write-Output ">>> Update .NETCore App location in Cmf.Foundation.Services.HostService.dll.config..."
& "$PSScriptRoot\EquipCreeUtils\UpdtNetCore3VersionRef\netcoreapp3.1\UpdtNetCore3VersionRef.exe" "$CreeRepo\LocalEnvironment\BusinessTier\Cmf.Foundation.Services.HostService.dll.config"

$password = ConvertTo-SecureString $DbPassword -AsPlainText -Force
$Cred = New-Object System.Management.Automation.PSCredential ($DbUser, $password)
do {
    Write-Output ">>> Copying online database..."
    & ".\Docker_CopyOnlineDB.ps1"
} while (-not(Test-SqlConnection -ServerName $DbServer -DatabaseName $DbName -Credential $Cred))

Set-Location "$CreeRepo\Business"

$MSBuildLocation = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe
Write-Output ">>> Found MSBuild at $MSBuildLocation"

Write-Output ">>> Downloading nuget.exe..."
$Client = New-Object Net.WebClient
$Client.DownloadFile("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe", "$CreeRepo\Business\nuget.exe")
& "$CreeRepo\Business\nuget.exe" restore "$SolutionFile"

Write-Output ">>> Building business solution..."
& "$MSBuildLocation" "$SolutionFile" /maxcpucount /p:Configuration="Debug" /p:Platform="Any CPU"

Write-Output ">>> Launching message bus..."
Start-Process -WorkingDirectory "$CreeRepo\LocalEnvironment\MessageBusGateway" "$CreeRepo\LocalEnvironment\MessageBusGateway\Cmf.MessageBus.Gateway.exe"

Write-Output ">>> Launching host..."
Start-Process -WorkingDirectory "$CreeRepo\LocalEnvironment\BusinessTier" "$CreeRepo\LocalEnvironment\BusinessTier\Cmf.Foundation.Services.HostService.exe"

Set-Location "$CreeRepo\UI\HTML"

Write-Output ">>> Building user interface..."
npm i
gulp install --update
gulp build

Write-Output ">>> Launching web server..."
Start-Process -WorkingDirectory "$CreeRepo\UI\HTML" "$GulpLocation" start

Write-Output ">>> Checking potential cmfpackage version updates..."
& "$PSScriptRoot\EquipCreeUtils\CheckPotentialPackageUptds\netcoreapp3.1\CheckPotentialPackageUptds.exe" "$CreeRepo"
