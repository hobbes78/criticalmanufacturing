################################################################################
##### Configurable Parameters

$CreeRepo = "C:\GitReps\CREE"

# --->>> Uncomment the equipment you intend to use <<<---
# $EquipName = "AmatCentura"
$EquipName = "AmatEndura"
# $EquipName = "AmatProducer"
# $EquipName = "AmatVerity2CDSem"
# $EquipName = "AmatVIIsta900"
# $EquipName = "CentrothermActivator"
# $EquipName = "FortrendSLPMVLSLP0100"
# $EquipName = "IVS280Inspectrology"
# $EquipName = "LaserTecSICA88"
# $EquipName = "LinkedLithographyLineScreenCanon"
# $EquipName = "KLAAltairCIRCL"
# $EquipName = "KlaF5X"
# $EquipName = "RigakuTXRF"
# $EquipName = "ScreenSU-20XXWetEtchTool"
# $EquipName = "ScreenVM32XXFilmThicknessMetrology"
# $EquipName = "ScreenZI-20XXDefectMetrology"
# $EquipName = "SemilabCnCV"
# $EquipName = "TelAlpha8SE"
# $EquipName = "TelAct8"
# $EquipName = "TelTrackACT8"

################################################################################
##### Script body

$AutomationController = $EquipName + "Controller"
if ($EquipName -eq "AmatVIIsta900") {
    $AutomationController = "AmatVIIstaController"
}

Write-Output ">>> Loading shared workflows..."
Start-Process -Wait 'cmd' -ArgumentList "/c $CreeRepo\LocalEnvironment\MasterDataLoader\MasterData.exe $CreeRepo\Features\TemplateAndSharedSolution\Data\MasterData\GenericWorkflows.xlsx /DeeBasePath:$CreeRepo\Data\DEEs /AutomationWorkflowFilesBasePath:$CreeRepo\Features\TemplateAndSharedSolution\Data\MasterData\AutomationWorkflowFiles /CreateInCollection:False"
Write-Output ">>> Loading equipment master data..."
# JSON
if (Test-Path -Path $CreeRepo\Features\$EquipName\Data\MasterData\$EquipName.json -PathType Leaf) {
    Start-Process -Wait 'cmd' -ArgumentList "/c $CreeRepo\LocalEnvironment\MasterDataLoader\MasterData.exe $CreeRepo\Features\$EquipName\Data\MasterData\$EquipName.json /DeeBasePath:$CreeRepo\Data\DEEs /AutomationWorkflowFilesBasePath:$CreeRepo\Features\$EquipName\Data\MasterData\AutomationWorkflowFiles /CreateInCollection:False"
}
# Excel
if (Test-Path -Path $CreeRepo\Features\$EquipName\Data\MasterData\$EquipName.xlsx -PathType Leaf) {
    Write-Output ">>> Warning: this equipment uses an Excel file as master data! Please change it to JSON"
    Start-Process -Wait 'cmd' -ArgumentList "/c $CreeRepo\LocalEnvironment\MasterDataLoader\MasterData.exe $CreeRepo\Features\$EquipName\Data\MasterData\$EquipName.xlsx /DeeBasePath:$CreeRepo\Data\DEEs /AutomationWorkflowFilesBasePath:$CreeRepo\Features\$EquipName\Data\MasterData\AutomationWorkflowFiles /CreateInCollection:False"
}

Write-Output ">>> Loading test master data..."
Start-Process -Wait 'cmd' -ArgumentList "/c $CreeRepo\LocalEnvironment\MasterDataLoader\MasterData.exe $CreeRepo\Tests\MasterData\MasterData-AutomationTests.json /DeeBasePath:$CreeRepo\Data\DEEs /CreateInCollection:False"

Set-Location "$CreeRepo\IoT\src\connect-iot-custom-utilities-cree-tasks"

Write-Output ">>> Building IoT custom utilities tasks..."
npm i
gulp install
gulp build

Write-Output ">>> SymLink for IoT custom utilities tasks..."
New-Item -ItemType SymbolicLink -Path "$CreeRepo\UI\HTML\apps\customization.web\node_modules\@criticalmanufacturing" -Name connect-iot-custom-utilities-cree-tasks -Value "$CreeRepo\IoT\src\connect-iot-custom-utilities-cree-tasks"

# Write-Output ">>> Creating a new version of automation controller..."
# & "$PSScriptRoot\EquipCreeUtils\CtrlNewVersion\netcoreapp3.1\CtrlNewVersion.exe" "$AutomationController"

# http://localhost:8083/api/ConnectIoT/FullUpdateAutomationControllerInstance
