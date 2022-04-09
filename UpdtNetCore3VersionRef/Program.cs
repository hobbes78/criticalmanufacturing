using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace UpdtNetCore3VersionRef
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Path to config file not specified!");
                return;
            }

            string pat = @"3\.1\.(\d+)"; // We're looking for version 3.1.x
            Regex r = new Regex(pat);

            string progFilesDir = Environment.GetEnvironmentVariable("ProgramFiles");
            string netCoreAppDir = Path.Combine(progFilesDir, @"dotnet\shared\Microsoft.NETCore.App");
            string[] subdirectoryEntries = Directory.GetDirectories(netCoreAppDir);
            int latestVersion = Int32.MinValue;
            string fullPath = null;
            foreach (string subdirectory in subdirectoryEntries)
            {
                string subdirName = Path.GetFileName(subdirectory);
                Match m = r.Match(subdirName);
                while (m.Success)
                {
                    Group g = m.Groups[1];
                    int thisVersion = Convert.ToInt32(g.ToString());
                    if (latestVersion < thisVersion)
                    {
                        latestVersion = thisVersion;
                        fullPath = subdirectory;
                    }
                    m = m.NextMatch();
                }
            }
            if (fullPath == null)
            {
                Console.WriteLine("No 3.1.x version of .NET Core was found!");
                return;
            }

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(args[0]);

            XmlNode node = xmlDoc.SelectSingleNode("configuration/appSettings/add[@key='MicrosoftNetPath']");
            node.Attributes["value"].Value = fullPath;

            xmlDoc.Save(args[0]);
        }
    }
}
