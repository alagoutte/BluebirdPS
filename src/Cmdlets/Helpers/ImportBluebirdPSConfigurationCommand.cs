using System.Management.Automation;
using System.IO;
using BluebirdPS.Core;

namespace BluebirdPS.Cmdlets.Helpers
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
                //Metadata.Configuration = Parsers.ConvertFromJson<Configuration>(File.ReadAllText(Metadata.Configuration.ConfigurationPath));
            }
            else
            {
                Metadata.Configuration = new Configuration();
            }
        }
    }
}
