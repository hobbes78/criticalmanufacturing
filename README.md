This is a collection of .NET utilities that can be used with [Critical Manufacturing MES](https://www.criticalmanufacturing.com/).

# CheckPotentialPackageUptds

This utility analyzes a project, listing all packages (i.e., `cmfpackage.json` files) that have dependencies for which a new version is available. To run it, just specify the directory to analyze as the sole command line argument:

```powershell
PS:\> .\CheckPotentialPackageUptds.exe C:\GitReps\CREE
C:\GitReps\CREE\Features\AmatCentura\cmfpackage.json: dependencies Cmf.Custom.Package.GenericCommonEquipmentIntegration is on version 1.3.0 when it could be on version 1.4.0
C:\GitReps\CREE\Features\AmatEndura\cmfpackage.json: dependencies Cmf.Custom.IoT is on version 5.0.0 when it could be on version 5.2.0
```

# CtrlNewVersion

Creates a new version of an **AutomationController**. It also creates a **ChangeSet** in the context of with the creation is performed; in the end the new version doesn't become effective, which leaves the controller in the **Created** state, thereby allowing changes. To run it, just specify the directory to analyze as the sole command line argument:

```powershell
PS:\> .\CtrlNewVersion.exe AmatCenturaController
```

Required settings must be specified in the **ClientConfiguration** instance that is created in the `Main()` method:

```csharp
ClientConfigurationProvider.ConfigurationFactory = () => new ClientConfiguration()
{
    HostAddress = "localhost:8083",
    ClientTenantName = "CREE",
    UseSsl = false,
    //SecurityToken = "4n5g84uLK347",
    ApplicationName = "EquipCree",
    IsUsingLoadBalancer = false,
    ThingsToDoAfterInitialize = null,
    UserName = @"CMF\cmfsu",
    Password = "qaz123WSX",
    RequestTimeout = "00:10:00",
};
```

# DocGenerator

This utility generates documentation in the context of IoT equipment integration. Specifically, it takes the equipment's MasterData JSON file and generates the **Configuration** markdown document.

```csharp
static void Main(string[] args)
{
    string repoPath = @"C:\GitReps\CREE";
    string equipmentName = "AmatVerity2CDSem";
    string package = "criticalmanufacturing/connect-iot-driver-secsgem";
```

To configure it, just edit the first three lines of the `Main()` method:

  * Specify the the directory path of target project repository in the first.
  * Specify the name of the equipment in the second.
  * Finally, enter the protocol package to be used in the third line.

If then you run it, a file will be generated in the expected location:

```
{repoPath}\UI\Help\src\packages\cmf.docs.area.cree\assets\TechSpec\connectiot\iotequipmenttypes\{equipmentName}\{equipmentName}-Configuration.md
```

# UpdtNetCore3VersionRef

This utility determines the system's latest .NET Core 3.1.x version available, and updates the project's config to use it. The advantage is that whenever a new version gets installed (or the currently used version gets uninstalled) via Windows, Visual Studio or Chocolatey update, everything just keeps working, if its execution becomes part of the development workflow. To run it, just specify the filepath of the config to update as the sole command line argument:

```powershell
PS:\> .\UpdtNetCore3VersionRef.exe C:\GitReps\CREE\LocalEnvironment\BusinessTier\Cmf.Foundation.Services.HostService.dll.config
```

# CleanCree.ps1

This script can be run upon the start of each user story. It cleans the whole environment, rebuilds everything in a few minutes, launches the message bus, host and web server.

# EquipCree.ps1

This script loads the IoT Template shared workflows, a specific equipment's workflows and test master data, builds and creates a symlink to IoT custom utilities tasks. Experimental support for the creation of a new version of the controller (in **Created** state) is available.
