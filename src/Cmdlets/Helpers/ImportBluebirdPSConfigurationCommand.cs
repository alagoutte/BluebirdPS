using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using BluebirdPS;
using System.IO;
using Newtonsoft;
using Newtonsoft.Json;

namespace BluebirdPS.Cmdlets
{
    [Cmdlet(VerbsData.Import, "BluebirdPSConfiguration")]
    public class ImportBluebirdPSConfigurationCommand : PSCmdlet
    {
        readonly string _fileDescription = "BluebirdPS configuration file";

        protected override void EndProcessing()
        {
            WriteVerbose($"Checking for {_fileDescription}.");

            if (File.Exists(Metadata.Configuration.ConfigurationPath))
            {
                WriteVerbose($"Importing {_fileDescription}.");
                Metadata.Configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(Metadata.Configuration.ConfigurationPath));                
            }
            else
            {
                Metadata.Configuration = new Configuration();
            }            
        }        
    }
}
