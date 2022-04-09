using Cmf.Custom.TestUtilities;
using Cmf.Foundation.BusinessObjects;
using Cmf.Foundation.BusinessOrchestration.ConnectIoTManagement.InputObjects;
using Cmf.Foundation.BusinessOrchestration.ConnectIoTManagement.OutputObjects;
using Cmf.LightBusinessObjects.Infrastructure;
using Cmf.TestScenarios.Others;
using System;

namespace CtrlNewVersion
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Automation controller name not specified!");
                return;
            }

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

            AutomationController autoCtrl = new AutomationController { Name = args[0] };
            if (!autoCtrl.ObjectExists())
            {
                Console.Error.WriteLine($"Automation controller '{args[0]}' not found!");
                return;
            }

            // Load object
            autoCtrl.Load();

            // Load workflows
            LoadAutomationControllerItemsInput lacii = new LoadAutomationControllerItemsInput
            {
                AutomationController = autoCtrl,
                LevelsToLoad = 0,
                OperationAttributes = new OperationAttributeCollection(),
            };
            LoadAutomationControllerItemsOutput lacio = lacii.LoadAutomationControllerItemsSync();
            autoCtrl = lacio.AutomationController;

            // Create change set
            autoCtrl.ChangeSet = new ChangeSet
            {
                Name = Guid.NewGuid().ToString(),
                Type = "General"
            };
            autoCtrl.ChangeSet.Create();

            // Create new version of controller
            autoCtrl = GenericServices.CreateObjectVersion(autoCtrl, false, null);
        }
    }
}
