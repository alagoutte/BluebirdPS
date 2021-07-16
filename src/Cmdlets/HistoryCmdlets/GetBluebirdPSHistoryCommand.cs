using System.Collections.Generic;
using System.Management.Automation;
using BluebirdPS.Models;

namespace BluebirdPS.Cmdlets.HistoryCmdlets
{
    [Cmdlet(VerbsCommon.Get, "BluebirdPSHistory")]
    [OutputType(typeof(List<ResponseData>))]
    public class GetBluebirdPSHistoryCommand : BaseCmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(History, true);
        }
    }
}
