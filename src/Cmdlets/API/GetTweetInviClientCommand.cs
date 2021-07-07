using BluebirdPS.Cmdlets.Base;
using System.Management.Automation;

namespace BluebirdPS.Cmdlets.API
{
    [Cmdlet(VerbsCommon.Get, "TweetInviClient")]
    public class GetTweetInviClientCommand : BluebirdPSClientCmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(client);
        }
    }
}
