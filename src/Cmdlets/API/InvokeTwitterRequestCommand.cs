using System.Management.Automation;
using System.Collections;
using System.Net;
using System.Linq;
using BluebirdPS.Cmdlets.Base;
using BluebirdPS.Exceptions;
using BluebirdPS.Core;
using System.Collections.Generic;

namespace BluebirdPS.Cmdlets
{
    [Cmdlet(VerbsLifecycle.Invoke, "TwitterRequest")]
    public class InvokeTwitterRequestCommand : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [ValidateObjectNotNullOrEmpty()]
        public TwitterRequest RequestParameters { get; set; }        
        
        protected override void ProcessRecord()
        {
            string errorId = RequestParameters.CommandName;

            if (RequestParameters.Body != null && RequestParameters.ContentType == "application/json")
            {
                if (!Parsers.IsJson(RequestParameters.Body))
                {
                    ThrowTerminatingError(new ErrorRecord(new BluebirdPSInvalidArgumentException("Parameter Body is not valid JSON"), errorId,ErrorCategory.InvalidArgument,"Request Body"));
                }
            }

            BluebirdPS.Authentication authentication = null;
            switch (RequestParameters.OAuthVersion)
            {
                case OAuthVersion.OAuth1a:
                    authentication = new BluebirdPS.Authentication(
                        RequestParameters,
                        Metadata.OAuth.ApiKey,
                        Metadata.OAuth.ApiSecret,
                        Metadata.OAuth.AccessToken,
                        Metadata.OAuth.AccessTokenSecret);
                    break;

                case OAuthVersion.OAuth2Bearer:
                    authentication = new BluebirdPS.Authentication(
                        RequestParameters,
                        Metadata.OAuth.BearerToken);
                    break;

                case OAuthVersion.Basic:
                    authentication = new BluebirdPS.Authentication(
                        RequestParameters,
                        Metadata.OAuth.ApiKey,
                        Metadata.OAuth.ApiSecret);
                    break;
            }

            using PowerShell pwsh = PowerShell.Create(RunspaceMode.CurrentRunspace);
            pwsh.AddCommand("Invoke-RestMethod")
                .AddParameter("Uri", authentication.Uri)
                .AddParameter("Method", authentication.HttpMethod)
                .AddParameter("Headers", new Hashtable() {
                        { "Authorization", authentication.AuthHeader },
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

            Metadata.LastResponseHeaders = BluebirdPS.Core.Helpers.GetVariable("ResponseHeaders").BaseObject;
            Metadata.LastStatusCode = (int)(HttpStatusCode)Core.Helpers.GetVariable("StatusCode").BaseObject;

            ResponseData twitterResponse = new ResponseData(RequestParameters, authentication, Metadata.LastResponseHeaders, (HttpStatusCode)Metadata.LastStatusCode, apiResponse);

            CheckRateLimit(twitterResponse);
            AddResponseToHistory(twitterResponse);

            if (Metadata.LastStatusCode == 401)
            {
                // throw error
            } else
            {
                WriteResponse(twitterResponse);
            }            
        }

        private void CheckRateLimit(ResponseData responseData)
        {
            string errorId = $"APIv{responseData.ApiVersion}-{responseData.Command}";

            if (responseData.RateLimitRemaining == 0)
            {
                string rateLimitMessage = $"Rate limit of {responseData.RateLimit} has been reached. Please wait until {responseData.RateLimitReset} before making another attempt for this resource.";
                BluebirdPSLimitsExceededException exception = new BluebirdPSLimitsExceededException(rateLimitMessage);
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
                        BluebirdPSLimitsExceededException exception = new BluebirdPSLimitsExceededException(rateLimitMessage);
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
                    string jsonOutput = Parsers.ConvertToJson(responseData.ApiResponse);
                    WriteObject(jsonOutput);
                    return;
                case OutputType.CustomClasses:
                    WriteCustomClasses(responseData);
                    break;
            }
        }

        private void WriteCustomClasses(ResponseData responseData)
        {
            switch (responseData.ApiVersion)
            {
                case "1.1":
                    if (responseData.Command != "Set-TwitterMutedUser")
                    {
                        foreach (object item in Parsers.ParseApiV1Response(responseData.ApiResponse)) {
                            WriteObject(item);
                        }
                    }
                    break;
                case "2":
                    foreach (object item in Parsers.ParseApiV2Response(responseData.ApiResponse))
                    {
                        WriteObject(item);
                    }
                    break;
                default:
                    WriteObject(responseData.ApiResponse);
                    break;
            }
        }
    }
}
