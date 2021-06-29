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
    [Cmdlet(VerbsData.Import, "BlulebirdPSConfiguration")]
    public class ImportBlulebirdPSConfiguration : PSCmdlet
    {
        readonly string _fileDescription = "BluebirdPS configuration file";

        protected override void EndProcessing()
        {
            WriteVerbose($"Checking {_fileDescription}.");

            if (File.Exists(Metadata.ConfigSavePath))
            {
                WriteVerbose($"Attempting to import {_fileDescription}.");
                Metadata.Configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(Metadata.ConfigSavePath));                
            }
            else
            {
                Metadata.Configuration = new Configuration();
            }
            
            WriteObject(Metadata.Configuration);
        }        
    }
}
