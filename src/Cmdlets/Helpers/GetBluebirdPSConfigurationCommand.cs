using BluebirdPS.Core;
using System.Management.Automation;

namespace BluebirdPS.Cmdlets.Helpers
{
    [Cmdlet(VerbsCommon.Get, "BluebirdPSConfiguration")]
    [OutputType(typeof(Configuration))]
    public class GetBluebirdPSConfigurationCommand : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(Metadata.Configuration);
        }        
    }
}
