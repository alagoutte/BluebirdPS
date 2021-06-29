using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using BluebirdPS;
using System.Net;
using System.Linq;

namespace BluebirdPS.Cmdlets
{
    [Cmdlet(VerbsLifecycle.Invoke, "TwitterRequest")]
    public class InvokeTwitterRequestCommand : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [ValidateObjectNotNullOrEmpty()]
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
                    .AddParameter("Headers", new Hashtable() { 
                        { "Authorization", _authentication.AuthHeader },
                        { "ContentType", RequestParameters.ContentType}
                    })
                    .AddParameter("ResponseHeadersVariable", "ResponseHeaders")
                    .AddParameter("StatusCodeVariable", "StatusCode")
                    .AddParameter("SkipHttpErrorCheck", true)
                    .AddParameter("Verbose", false);

                if (RequestParameters.Form != null)
                {
                    pwsh.AddParameter("Form", RequestParameters.Form);
                }
                if (RequestParameters.Body != null)
                {
                    pwsh.AddParameter("Body", RequestParameters.Body);
                }

                Collection<PSObject> apiResponse = pwsh.Invoke();
                pwsh.Commands.Clear();

                PSObject responseHeaders = Helpers.GetVariable("ResponseHeaders");
                HttpStatusCode statusCode = (HttpStatusCode)Helpers.GetVariable("StatusCode").BaseObject;

                ResponseData twitterResponse = new ResponseData(RequestParameters, _authentication, responseHeaders, statusCode, apiResponse);

                Helpers.SetVariable("Response", twitterResponse, "Global");
                WriteTwitterResponseCommand writeResponse = new WriteTwitterResponseCommand()
                {
                    ResponseData = twitterResponse
                };

                WriteObject(writeResponse.Invoke<object>());

            }
        }
    }
}
