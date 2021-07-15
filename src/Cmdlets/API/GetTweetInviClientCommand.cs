using System.Management.Automation;
using BluebirdPS.Cmdlets.Base;

namespace BluebirdPS.Cmdlets.API
{
    [Cmdlet(VerbsCommon.Get, "TweetInviClient")]
    public class GetTweetInviClientCommand : ClientCmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(Client);
        }
    }
}
