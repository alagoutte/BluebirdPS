using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using BluebirdPS.Cmdlets.Base;
using System.Collections;

namespace BluebirdPS.Cmdlets.APIV2
{
    /// <summary>
    /// 
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "TwitterUser")]
    public class GetTwitterUserCommand : BluebirdPSUserCmdlet
    {
        [Parameter(ValueFromPipeline = true)]
        [ValidateCount(1,100)]

        public List<string> User { get; set; }

        List<string> userNames = new List<string>();
        List<string> userIds = new List<string>();

        protected override void ProcessRecord()
        {
            foreach (string user in User)
            {
                try
                {
                    _= long.Parse(user);
                    userIds.Add(user);
                }
                catch
                {
                    userNames.Add(user);
                }
            }
        }

        protected override void EndProcessing()
        {
            TwitterRequest request;
            InvokeTwitterRequestCommand apiRequest;
            string commandName = MyInvocation.MyCommand.Name;
            if (userNames.Count == 0 && userIds.Count == 0)
            {
                request = new TwitterRequest()
                {
                    Endpoint = new Uri($"https://api.twitter.com/2/users/by/username/{Metadata.Configuration.AuthUserName}"),
                    IncludeExpansions = IncludeExpansions,
                    ExpansionType = ExpansionType,
                    CommandName = commandName
                };

                apiRequest = new InvokeTwitterRequestCommand()
                {
                    RequestParameters = request
                };
                WriteObject(apiRequest.Invoke<object>());
            }

            if (userNames.Count > 0)
            {
                request = new TwitterRequest()
                {
                    IncludeExpansions = IncludeExpansions,
                    ExpansionType = ExpansionType,
                    CommandName = commandName
                };
                if (userNames.Count == 1)
                {
                    request.Endpoint = new Uri($"https://api.twitter.com/2/users/by/username/{userNames.First()}");
                } else
                {
                    request.Endpoint = new Uri($"https://api.twitter.com/2/users/by");
                    request.Query = new Hashtable
                    {
                        { "usernames", string.Join(",", userNames) }
                    };
                }
                apiRequest = new InvokeTwitterRequestCommand()
                {
                    RequestParameters = request
                };
                WriteObject(apiRequest.Invoke<object>());
            }
            if (userIds.Count > 0)
            {
                request = new TwitterRequest()
                {
                    IncludeExpansions = IncludeExpansions,
                    ExpansionType = ExpansionType,
                    CommandName = commandName
                };
                if (userNames.Count == 1)
                {
                    request.Endpoint = new Uri($"https://api.twitter.com/2/users/{userIds.First()}");
                }
                else
                {
                    request.Endpoint = new Uri($"https://api.twitter.com/2/users");
                    request.Query = new Hashtable
                    {
                        { "usernames", string.Join(",", userIds) }
                    };
                }
                apiRequest = new InvokeTwitterRequestCommand()
                {
                    RequestParameters = request
                };
                WriteObject(apiRequest.Invoke<object>());
            }
        }

    }
}
