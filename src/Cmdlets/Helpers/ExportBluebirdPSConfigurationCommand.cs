using BluebirdPS.Cmdlets.Base;
using BluebirdPS.Core;
using System.IO;
using System.Management.Automation;
namespace BluebirdPS.Cmdlets.Helpers
{
    [Cmdlet(VerbsData.Export, "BluebirdPSConfiguration")]
    public class ExportBluebirdPSConfigurationCommand : BluebirdPSCmdlet
    {
        protected override void ProcessRecord()
        {
            string _action = File.Exists(configuration.ConfigurationPath) ? "existing" : "new";

            if (File.Exists(configuration.CredentialsPath))
            {
                configuration.AuthLastExportDate = File.GetLastWriteTime(configuration.CredentialsPath);
            }

            string message = $"Saved BluebirdPS Configuration to {_action} file: {configuration.ConfigurationPath}";
            WriteVerbose(message);

            Config.ExportConfiguration();
        }
    }
}
