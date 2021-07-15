using System.Collections.Generic;
using System.Management.Automation;
using BluebirdPS.Cmdlets.Base;
using BluebirdPS.Models;
namespace BluebirdPS.Cmdlets.Helpers
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
