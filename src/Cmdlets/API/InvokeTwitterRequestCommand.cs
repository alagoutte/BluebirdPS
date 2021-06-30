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

                PSObject apiResponse = pwsh.Invoke().ToList()[0];
                pwsh.Commands.Clear();
                
                var responseHeaders = Helpers.GetVariable("ResponseHeaders").BaseObject;
                HttpStatusCode statusCode = (HttpStatusCode)Helpers.GetVariable("StatusCode").BaseObject;
                Metadata.LastStatusCode = (int)statusCode;

                ResponseData twitterResponse = new ResponseData(RequestParameters, _authentication, responseHeaders, statusCode, apiResponse);

                CheckRateLimit(twitterResponse);
                AddResponseToHistory(twitterResponse);
                WriteResponse(twitterResponse);
            }
        }

        private void CheckRateLimit(ResponseData responseData)
        {
            string errorId = $"APIv{responseData.ApiVersion}-{responseData.Command}";

            if (responseData.RateLimitRemaining == 0)
            {
                string rateLimitMessage = $"Rate limit of {responseData.RateLimit} has been reached. Please wait until {responseData.RateLimitReset} before making another attempt for this resource.";
                LimitsExceededException exception = new LimitsExceededException(rateLimitMessage);
                ThrowTerminatingError(new ErrorRecord(exception, errorId, ErrorCategory.LimitsExceeded, responseData.Endpoint));
            }

            if (responseData.RateLimitRemaining < Metadata.Configuration.RateLimitThreshold)
            {
                string rateLimitMessage = $"The rate limit for this resource is {responseData.RateLimit}. There are {responseData.RateLimitRemaining} remaining calls to this resource until {responseData.RateLimitReset}";
                switch (Metadata.Configuration.RateLimitAction)
                {
                    case RateLimitAction.Verbose:
                        WriteVerbose(rateLimitMessage);
                        break;
                    case RateLimitAction.Warning:
                        WriteWarning(rateLimitMessage);
                        break;
                    case RateLimitAction.Error:
                        LimitsExceededException exception = new LimitsExceededException(rateLimitMessage);
                        WriteError(new ErrorRecord(exception, errorId, ErrorCategory.LimitsExceeded, responseData.Endpoint));
                        break;
                }
            }
        }

        private void AddResponseToHistory(ResponseData responseData)
        {
            Metadata.History.Add(responseData);
            WriteInformation(new InformationRecord(responseData,"BluebirdPS"));
        }

        private void WriteResponse(ResponseData responseData)
        {
            switch (Metadata.Configuration.OutputType)
            {
                case OutputType.PSCustomObject:
                    WriteObject(responseData.ApiResponse);
                    return;
                case OutputType.JSON:
                    string jsonOutput = JsonConvert.SerializeObject(responseData.ApiResponse, Formatting.Indented);
                    WriteObject(jsonOutput);
                    return;
                case OutputType.CustomClasses:
                    WriteCustomClasses(responseData.ApiResponse);
                    break;
            }
        }

        private void WriteCustomClasses(dynamic apiResponse)
        {

        }
    }
}
