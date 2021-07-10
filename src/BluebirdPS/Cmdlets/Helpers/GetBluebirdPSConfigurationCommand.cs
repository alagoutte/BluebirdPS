using BluebirdPS.Cmdlets.Base;
using BluebirdPS.Core;
using System.Management.Automation;

namespace BluebirdPS.Cmdlets.Helpers
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
