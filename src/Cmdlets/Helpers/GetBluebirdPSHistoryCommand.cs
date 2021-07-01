using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;

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
