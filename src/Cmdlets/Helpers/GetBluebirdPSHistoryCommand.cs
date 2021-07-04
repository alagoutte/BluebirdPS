using BluebirdPS.Cmdlets.Base;
using BluebirdPS.Core;
using System.Management.Automation;
namespace BluebirdPS.Cmdlets.Helpers
{
    [Cmdlet(VerbsCommon.Get, "BluebirdPSHistory")]
    //[OutputType(typeof(List<ResponseData>))]
    public class GetBluebirdPSHistoryCommand : BluebirdPSCmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(history, true);
            //WriteObject(Config.BeforeWaitingForRequestRateLimits, true);
            //WriteObject(Config.WaitingForRateLimit, true);
            //WriteObject(Config.BeforeExecutingRequest, true);
            WriteObject(Config.AfterExecutingRequest, true);
            WriteObject(Config.OnTwitterException, true);
        }
    }
}
