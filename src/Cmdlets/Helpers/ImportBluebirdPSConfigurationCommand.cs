using BluebirdPS.Cmdlets.Base;
using BluebirdPS.Core;
using System.Management.Automation;
namespace BluebirdPS.Cmdlets.Helpers
{
    [Cmdlet(VerbsData.Import, "BluebirdPSConfiguration")]
    public class ImportBluebirdPSConfigurationCommand : BluebirdPSCmdlet
    {
        protected override void ProcessRecord()
        {
            WriteVerbose($"Importing BluebirdPS configuration file.");
            Config.ImportConfiguration();
        }
    }
}
