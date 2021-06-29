using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.ObjectModel;

namespace BluebirdPS.Cmdlets
{
    [Cmdlet(VerbsLifecycle.Invoke, "TwitterRequest")]
    public class InvokeTwitterRequestCommand : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public TwitterRequest RequestParameters { get; set; }

        Authentication _authentication;

        protected override void ProcessRecord()
        {

            if (RequestParameters.Body != null && RequestParameters.ContentType == "application/json")
            {
                _ = JsonConvert.DeserializeObject(RequestParameters.Body);

            }

            switch (RequestParameters.OAuthVersion)
            {
                case OAuthVersion.OAuth1a:
                    _authentication = new Authentication(
                        RequestParameters,
                        Metadata.OAuth.ApiKey,
                        Metadata.OAuth.ApiSecret,
                        Metadata.OAuth.AccessToken,
                        Metadata.OAuth.AccessTokenSecret);
                    break;

                case OAuthVersion.OAuth2Bearer:
                    _authentication = new Authentication(
                        RequestParameters,
                        Metadata.OAuth.BearerToken);
                    break;

                case OAuthVersion.Basic:
                    _authentication = new Authentication(
                        RequestParameters,
                        Metadata.OAuth.ApiKey,
                        Metadata.OAuth.ApiSecret);
                    break;
            }

            using (PowerShell pwsh = PowerShell.Create(RunspaceMode.CurrentRunspace))
            {
                pwsh.AddCommand("Invoke-RestMethod")
                    .AddParameter("Uri", _authentication.Uri)
                    .AddParameter("Method", _authentication.HttpMethod)
                    .AddParameter("Headers", new Hashtable() { { "Authorization", _authentication.AuthHeader } })
                    .AddParameter("ContentType", RequestParameters.ContentType)
                    .AddParameter("ResponseHeadersVariable", "ResponseHeaders")
                    .AddParameter("StatusCodeVariable", "StatusCode")
                    .AddParameter("SkipHttpErrorCheck", true)
                    .AddParameter("Verbose", true);

                Collection<PSObject> apiResponse = pwsh.Invoke();

            }
        }
    }
}
