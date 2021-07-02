using System.Collections.Generic;
using System.Management.Automation;
using BluebirdPS.Core;
using BluebirdPS.Models;

namespace BluebirdPS.Cmdlets.Helpers
{
    [Cmdlet(VerbsCommon.Get, "BluebirdPSHistory")]
    [OutputType(typeof(List<ResponseData>))]
    public class GetBluebirdPSHistoryCommand : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(Metadata.History);
        }
    }
}
