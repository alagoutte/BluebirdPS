using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using BluebirdPS;
using System.Management.Automation.Runspaces;

namespace BluebirdPS.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "TwitterUser")]
    public class GetTwitterUserCommand : BluebirdPSUserCmdlet
    {
        protected override void ProcessRecord()
        {
            TwitterRequest request = new TwitterRequest()
            {
                Endpoint = new Uri($"https://api.twitter.com/2/users/by/username/thedavecarroll"),
                IncludeExpansions = IncludeExpansions,
                ExpansionType = ExpansionType
            };

            InvokeTwitterRequestCommand apiRequest = new InvokeTwitterRequestCommand()
            {
                RequestParameters = request
            };

            WriteObject(apiRequest.Invoke<object>());
        }

    }
}
