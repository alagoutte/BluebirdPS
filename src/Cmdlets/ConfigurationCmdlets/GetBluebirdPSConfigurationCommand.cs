using System.Management.Automation;
using Configuration = BluebirdPS.Models.Configuration;

namespace BluebirdPS.Cmdlets.ConfigurationCmdlets
{
    [Cmdlet(VerbsCommon.Get, "BluebirdPSConfiguration")]
    [OutputType(typeof(Configuration))]
    public class GetBluebirdPSConfigurationCommand : BaseCmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(Configuration);
        }
    }
}
