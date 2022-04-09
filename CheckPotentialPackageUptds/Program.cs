using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CheckPotentialPackageUptds
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Cree code repository not specified!");
                return;
            }

            Dictionary<string, string> currVersions = new Dictionary<string, string>();
            ProcessCmfPackages(args[0], (JsonDocument jsonCmfPackage, string filepath) =>
            {
                string packageId = jsonCmfPackage.RootElement.GetProperty("packageId").GetString();
                string version = jsonCmfPackage.RootElement.GetProperty("version").GetString();
                currVersions[packageId] = version;
            });

            CheckVersions(args[0], currVersions, "dependencies");
            CheckVersions(args[0], currVersions, "testPackages");
        }

        private static void CheckVersions(string CreeRepo, Dictionary<string, string> currVersions, string whatToCheck)
        {
            ProcessCmfPackages(CreeRepo, (JsonDocument jsonCmfPackage, string filepath) =>
            {
                if (jsonCmfPackage.RootElement.TryGetProperty(whatToCheck, out JsonElement dependencies))
                {
                    foreach (JsonElement dependency in dependencies.EnumerateArray())
                    {
                        string id = dependency.GetProperty("id").GetString();
                        string version = dependency.GetProperty("version").GetString();
                        if (currVersions[id] != version)
                        {
                            Console.WriteLine($"{filepath}: {whatToCheck} {id} is on version {version} when it could be on version {currVersions[id]}");
                        }
                    }
                }
            });
        }

        private static void ProcessCmfPackages(string CreeRepo, Action<JsonDocument, string> action)
        {
            var cmfpackageFiles = Directory.EnumerateFiles(CreeRepo, "cmfpackage.json", SearchOption.AllDirectories);
            foreach (string cmfpackageFile in cmfpackageFiles)
            {
                using (var fsCmfPackage = new FileStream(cmfpackageFile, FileMode.Open))
                using (JsonDocument jsonCmfPackage = JsonDocument.Parse(fsCmfPackage))
                {
                    action(jsonCmfPackage, cmfpackageFile);
                }
            }
        }
    }
}
