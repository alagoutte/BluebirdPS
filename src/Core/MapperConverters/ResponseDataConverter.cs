using AutoMapper;
using BluebirdPS.Models;
using System;
using System.Linq;
using System.Management.Automation;
using Tweetinvi.Events;

namespace BluebirdPS.Core.MapperConverters
{
    internal class ResponseDataConverter : ITypeConverter<AfterExecutingQueryEventArgs, ResponseData>
    {
        public ResponseData Convert(AfterExecutingQueryEventArgs source, ResponseData destination, ResolutionContext context)
        {
            destination = new ResponseData();

            try
            {
                // this does not work
                using PowerShell pwsh = PowerShell.Create(RunspaceMode.NewRunspace);
                CallStackFrame _callStackFrame = pwsh.Runspace.Debugger.GetCallStack().ToList().First();
                destination.Command = _callStackFrame.InvocationInfo.MyCommand.Name;
                destination.InvocationInfo = _callStackFrame.InvocationInfo;
            }
            catch { }

            Uri endpointUri = new Uri(source.Url);

            destination.Timestamp = source.CompletedDateTime.DateTime;
            destination.HttpMethod = (HttpMethod)source.TwitterQuery.HttpMethod;
            destination.Uri = endpointUri;
            destination.Endpoint = endpointUri.AbsolutePath;
            destination.QueryString = endpointUri.Query;
            //destination.Body = null;
            //destination.Form = null;
            destination.OAuthVersion = OAuthVersion.OAuth1a;
            //destination.Status = null;
            if (source.HttpHeaders.ContainsKey("Server"))
            {
                destination.Server = source.HttpHeaders["Server"].First();
            }

            if (source.HttpHeaders.ContainsKey("x-response-time"))
            {
                destination.ResponseTime = int.Parse(source.HttpHeaders["x-response-time"].First());
            }

            destination.RateLimit = source.QueryRateLimit.Limit;
            destination.RateLimitRemaining = source.QueryRateLimit.Remaining;
            destination.RateLimitReset = source.QueryRateLimit.ResetDateTime;
            destination.HeaderResponse = source.HttpHeaders;
            destination.ApiVersion = endpointUri.Segments[1].Trim('/');
            destination.ApiResponse = source.HttpContent;

            return destination;
        }
    }
}
