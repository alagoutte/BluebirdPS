using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.IO;
using BluebirdPS;
using Newtonsoft.Json;

namespace BluebirdPS.Cmdlets
{
    [Cmdlet(VerbsData.Export, "BluebirdPSConfiguration")]
    public class ExportBluebirdPSConfigurationCommand : Cmdlet
    {
        protected override void ProcessRecord()
        {
            string _action = File.Exists(Metadata.Configuration.ConfigurationPath) ? "existing" : "new";
            
            if (File.Exists(Metadata.Configuration.CredentialsPath))
            {
                Metadata.Configuration.AuthLastExportDate = File.GetLastWriteTime(Metadata.Configuration.CredentialsPath);
            }

            string message = $"Saved BluebirdPS Configuration to {_action} file: {Metadata.Configuration.ConfigurationPath}";
            WriteVerbose(message);

            string _configuration = JsonConvert.SerializeObject(Metadata.Configuration,Formatting.Indented);
            WriteObject(_configuration);
        }
        
    }
}
