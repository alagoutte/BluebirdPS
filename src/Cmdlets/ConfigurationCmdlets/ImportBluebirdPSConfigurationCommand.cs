using System.Management.Automation;
using BluebirdPS.Core;

namespace BluebirdPS.Cmdlets.ConfigurationCmdlets
{
    [Cmdlet(VerbsData.Import, "BluebirdPSConfiguration")]
    public class ImportBluebirdPSConfigurationCommand : BaseCmdlet
    {
        protected override void ProcessRecord()
        {
            WriteVerbose($"Importing BluebirdPS configuration file.");
            Config.ImportConfiguration();
        }
    }
}
