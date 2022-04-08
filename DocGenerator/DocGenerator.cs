using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace DocGenerator
{
    class DocGenerator
    {
        static void Main(string[] args)
        {
            string repoPath = @"C:\GitReps\CREE";
            string equipmentName = "AmatVerity2CDSem";
            string package = "criticalmanufacturing/connect-iot-driver-secsgem";

            string MasterDataPath = Path.Combine(repoPath, @$"Features\{equipmentName}\Data\MasterData\{equipmentName}.json");
            string MarkDownPath = Path.Combine(repoPath, $@"UI\Help\src\packages\cmf.docs.area.cree\assets\TechSpec\connectiot\iotequipmenttypes\{equipmentName}\{equipmentName}-Configuration.md");

            AutomationProperty[] aAutomationProperties = null;
            AutomationEvent[] aAutomationEvents = null;
            AutomationEventProperties[] aAutomationEventProperties = null;
            AutomationCommand[] aAutomationCommands = null;
            AutomationCommandParameter[] aAutomationCommandParameters = null;
            AutomationController[] aAutomationControllers = null;
            AutomationDriverDefinition[] aAutomationDriverDefinitions = null;
            AutomationControllerDriverDef[] aAutomationControllerDriverDefs = null;

            using (var masterdata = new FileStream(MasterDataPath, FileMode.Open))
            {
                using (JsonDocument document = JsonDocument.Parse(masterdata))
                {
                    string strAutomationProperties = document.RootElement.GetProperty("AutomationProperty").GetRawText();
                    Dictionary<string, AutomationProperty> dicAutomationProperties
                        = JsonSerializer.Deserialize<Dictionary<string, AutomationProperty>>(strAutomationProperties);
                    aAutomationProperties = dicAutomationProperties.OrderBy(kvp => Int32.Parse(kvp.Key)).Select(kvp => kvp.Value).ToArray();

                    string strAutomationEvents = document.RootElement.GetProperty("AutomationEvent").GetRawText();
                    Dictionary<string, AutomationEvent> dicAutomationEvents
                        = JsonSerializer.Deserialize<Dictionary<string, AutomationEvent>>(strAutomationEvents);
                    aAutomationEvents = dicAutomationEvents.OrderBy(kvp => Int32.Parse(kvp.Key)).Select(kvp => kvp.Value).ToArray();

                    string strAutomationEventProperties = document.RootElement.GetProperty("AutomationEventProperties").GetRawText();
                    Dictionary<string, AutomationEventProperties> dicAutomationEventProperties
                        = JsonSerializer.Deserialize<Dictionary<string, AutomationEventProperties>>(strAutomationEventProperties);
                    aAutomationEventProperties = dicAutomationEventProperties.OrderBy(kvp => Int32.Parse(kvp.Key)).Select(kvp => kvp.Value).ToArray();

                    string strAutomationCommands = document.RootElement.GetProperty("AutomationCommand").GetRawText();
                    Dictionary<string, AutomationCommand> dicAutomationCommands
                        = JsonSerializer.Deserialize<Dictionary<string, AutomationCommand>>(strAutomationCommands);
                    aAutomationCommands = dicAutomationCommands.OrderBy(kvp => Int32.Parse(kvp.Key)).Select(kvp => kvp.Value).ToArray();

                    string strAutomationCommandParameters = document.RootElement.GetProperty("AutomationCommandParameter").GetRawText();
                    Dictionary<string, AutomationCommandParameter> dicAutomationCommandParameters
                        = JsonSerializer.Deserialize<Dictionary<string, AutomationCommandParameter>>(strAutomationCommandParameters);
                    aAutomationCommandParameters = dicAutomationCommandParameters.OrderBy(kvp => Int32.Parse(kvp.Key)).Select(kvp => kvp.Value).ToArray();

                    string strAutomationControllers = document.RootElement.GetProperty("<DM>AutomationController").GetRawText();
                    Dictionary<string, AutomationController> dicAutomationControllers
                        = JsonSerializer.Deserialize<Dictionary<string, AutomationController>>(strAutomationControllers);
                    aAutomationControllers = dicAutomationControllers.OrderBy(kvp => Int32.Parse(kvp.Key)).Select(kvp => kvp.Value).ToArray();

                    string strAutomationDriverDefinitions = document.RootElement.GetProperty("<DM>AutomationDriverDefinition").GetRawText();
                    Dictionary<string, AutomationDriverDefinition> dicAutomationDriverDefinitions
                        = JsonSerializer.Deserialize<Dictionary<string, AutomationDriverDefinition>>(strAutomationDriverDefinitions);
                    aAutomationDriverDefinitions = dicAutomationDriverDefinitions.OrderBy(kvp => Int32.Parse(kvp.Key)).Select(kvp => kvp.Value).ToArray();

                    string strAutomationControllerDriverDefs = document.RootElement.GetProperty("AutomationControllerDriverDef").GetRawText();
                    Dictionary<string, AutomationControllerDriverDef> dicAutomationControllerDriverDefs
                        = JsonSerializer.Deserialize<Dictionary<string, AutomationControllerDriverDef>>(strAutomationControllerDriverDefs);
                    aAutomationControllerDriverDefs = dicAutomationControllerDriverDefs.OrderBy(kvp => Int32.Parse(kvp.Key)).Select(kvp => kvp.Value).ToArray();
                }
            }

            Directory.CreateDirectory(Path.GetDirectoryName(MarkDownPath));
            using (var markdown = new StreamWriter(MarkDownPath, false, Encoding.UTF8))
            {
                markdown.WriteLine("Configuration");
                markdown.WriteLine("============");
                markdown.WriteLine($"This section describe the setup for {equipmentName} Equipment Type");
                markdown.WriteLine();

                markdown.WriteLine("Protocol");
                markdown.WriteLine("========");
                markdown.WriteLine($"The protocol used by the Automation Driver referenced on the Automation Controller is named **{aAutomationDriverDefinitions.First().AutomationProtocol}** and used the out-of-the-box package **{package}**, using version **{aAutomationControllers.First().ControllerPackageVersion}** at the time of writing.");
                markdown.WriteLine();

                markdown.WriteLine("Driver Definition");
                markdown.WriteLine("=================");
                markdown.WriteLine($"The Automation Driver referenced on the Automation Controller as **{aAutomationControllerDriverDefs.First().Name}** is the Automation Driver **{aAutomationProperties.First().AutomationDriverDefinition}**, this driver contains the information regarding all the items needed for the automation of the equipment.");
                markdown.WriteLine();

                markdown.WriteLine("Properties");
                markdown.WriteLine("==========");
                markdown.WriteLine();
                markdown.WriteLine("The table below describes the workflow properties");
                markdown.WriteLine();
                markdown.WriteLine("Name | ID | Type | Equipment Type | Description ");
                markdown.WriteLine(":------------ | -------: | :-------- | :---------- | :-------- ");
                foreach (var ap in aAutomationProperties)
                {
                    markdown.WriteLine($" {ap.Name} | { ap.DevicePropertyId } | {ap.DataType} | {ap.AutomationProtocolDataType} | {ap.Description}");
                }
                markdown.WriteLine();

                markdown.WriteLine("Events");
                markdown.WriteLine("======");
                markdown.WriteLine();
                foreach (var ae in aAutomationEvents)
                {
                    markdown.WriteLine($"### {ae.Name}");
                    markdown.WriteLine();
                    if (String.IsNullOrEmpty(ae.Description))
                    {
                        markdown.Write($"Event for {ae.Name}.");
                    }
                    else
                    {
                        markdown.Write($"Event for {ae.Description}.");
                    }
                    if (String.IsNullOrEmpty(ae.DeviceEventId))
                    {
                        markdown.WriteLine();
                    }
                    else
                    {
                        markdown.WriteLine($" ID: **{ae.DeviceEventId}**");
                    }
                    var thisAE = aAutomationEventProperties.Where(a => a.AutomationEvent == ae.Name);
                    if (thisAE.Any())
                    {
                        markdown.WriteLine("#### *Event Properties*");
                        markdown.WriteLine();
                        markdown.WriteLine("Name          | ID | Type      | Is Mandatory | Equipment Data Type | Description");
                        markdown.WriteLine(":------------ | :- | :-------- | :----------: | :-------- | :-----------");
                        foreach (var aep in thisAE)
                        {
                            var ap = aAutomationProperties.Single(a => a.Name == aep.AutomationProperty);
                            markdown.WriteLine($" {aep.AutomationProperty} | {ap.DevicePropertyId} | {ap.DataType} | Yes | {ap.AutomationProtocolDataType} | {ap.Description}");
                        }
                    }
                    else
                    {
                        markdown.WriteLine();
                        markdown.WriteLine("*(this event has no properties)*");
                    }
                    markdown.WriteLine();
                }

                markdown.WriteLine("Commands");
                markdown.WriteLine("========");
                markdown.WriteLine();
                if (aAutomationCommands.Length > 0)
                {
                    foreach (var ac in aAutomationCommands)
                    {
                        markdown.WriteLine($"### {ac.Name}");
                        markdown.WriteLine();
                        markdown.Write($"Command for {ac.Description}.");
                        if (String.IsNullOrEmpty(ac.DeviceCommandId))
                        {
                            markdown.WriteLine();
                        }
                        else
                        {
                            markdown.WriteLine($" ID: **{ac.DeviceCommandId}**");
                        }
                        var thisACparams = aAutomationCommandParameters.Where(a => a.AutomationCommand == ac.Name);
                        if (thisACparams.Any())
                        {
                            markdown.WriteLine();
                            markdown.WriteLine("#### *Command Parameters*");
                            markdown.WriteLine();
                            markdown.WriteLine("Name          | Is Mandatory | Equipment Data Type | Description");
                            markdown.WriteLine(":------------ | :- | :-------- | :----------");
                            foreach (var param in thisACparams)
                            {
                                string isMandatory = (param.IsMandatory == "True") ? "Yes" : "No";
                                markdown.WriteLine($" {param.Name} | { isMandatory } | {param.AutomationProtocolDataType} | {param.Description}");
                            }
                        }
                        else
                        {
                            markdown.WriteLine();
                            markdown.WriteLine("*(this command has no parameters)*");
                        }
                        markdown.WriteLine();
                    }
                }
                else
                {
                    markdown.WriteLine("No remote command is used for this Equipment Integration");
                }
            }
        }
    }
    public class AutomationProperty
    {
        public string AutomationDriverDefinition { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DevicePropertyId { get; set; }
        public string IsReadable { get; set; }
        public string IsWritable { get; set; }
        public string DataType { get; set; }
        public string AutomationProtocolDataType { get; set; }
        public string ExtendedData { get; set; }
    }

    public class AutomationEvent
    {
        public string AutomationDriverDefinition { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DeviceEventId { get; set; }
        public bool IsEnabled { get; set; }
        public string ExtendedData { get; set; }
    }

    public class AutomationEventProperties
    {
        public string AutomationDriverDefinition { get; set; }
        public string AutomationEvent { get; set; }
        public string AutomationProperty { get; set; }
        public int Order { get; set; }
        public string ExtendedData { get; set; }
    }

    public class AutomationCommand
    {
        public string AutomationDriverDefinition { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DeviceCommandId { get; set; }
        public string ExtendedData { get; set; }
    }

    public class AutomationCommandParameter
    {
        public string AutomationDriverDefinition { get; set; }
        public string AutomationCommand { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DataType { get; set; }
        public string AutomationProtocolDataType { get; set; }
        public string DefaultValue { get; set; }
        public string IsMandatory { get; set; }
        public string Order { get; set; }
        public string ExtendedData { get; set; }
    }

    public class AutomationController
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string ControllerPackageVersion { get; set; }
        public string ObjectType { get; set; }
        public string TasksPackages { get; set; }
        public string Scope { get; set; }
    }

    public class AutomationDriverDefinition
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string AutomationProtocol { get; set; }
        public string ObjectType { get; set; }
    }
    public class AutomationControllerDriverDef
    {
        public string AutomationController { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string AutomationDriverDefinition { get; set; }
        public string Color { get; set; }
    }
}
