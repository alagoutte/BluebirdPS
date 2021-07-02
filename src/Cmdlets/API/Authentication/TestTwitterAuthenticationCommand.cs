using System;
using System.Collections.Generic;
using System.Text;
using BluebirdPS.Cmdlets.Base;
using System.Management.Automation;
using BluebirdPS.Exceptions;

namespace BluebirdPS.Cmdlets.API.Authentication
{
    [Cmdlet(VerbsDiagnostic.Test, "TwitterAuthentication")]
    public class TestTwitterAuthenticationCommand : PSCmdlet
    {
        [Parameter()]
        public SwitchParameter BearerToken { get; set; }

        protected override void ProcessRecord()
        {
            TwitterRequest request;
            if (BearerToken.IsPresent)
            {
                request = new TwitterRequest()
                {
                    OAuthVersion = OAuthVersion.OAuth2Bearer,
                    Endpoint = new Uri($"https://api.twitter.com/2/users/{Metadata.Configuration.AuthUserId}")
                };

            } else
            {
                request = new TwitterRequest()
                {
                    Endpoint = new Uri($"https://api.twitter.com/1.1/account/verify_credentials.json"),
                    Query = new System.Collections.Hashtable()
                    {
                        { "include_entities", "false" },
                        { "skip_status", "true" }
                    }
                };
            }

            try
            {
                InvokeTwitterRequestCommand apiRequest = new InvokeTwitterRequestCommand()
                {
                    RequestParameters = request
                };
                _ = apiRequest.Invoke();

                Metadata.Configuration.AuthValidationDate = DateTime.Now;
                WriteObject(true);
            }
            catch
            {
                Metadata.Configuration.AuthValidationDate = null;
                WriteObject(false);
            }
        }

    }
}
