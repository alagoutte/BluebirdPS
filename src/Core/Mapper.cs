using AutoMapper;
using Tweetinvi.Events;
using BluebirdPS.Models;
using Tweetinvi.Models.V2;
using BluebirdPS.Models.APIV2;
using BluebirdPS.Models.APIV2.Metrics.User;
using System;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace BluebirdPS.Core
{
    internal class BluebirdPSMapper
    {
        private static IMapper mapper;
        public static IMapper GetOrCreateInstance() => mapper ??= Create();
        private static IMapper Create()
        {
            MapperConfiguration mapperConfig = new MapperConfiguration(cfg => {
                cfg.CreateMap<DateTimeOffset, DateTime>().ConvertUsing(new DateTimeConverter());
                cfg.CreateMap<UserV2, User>()
                    .ForMember("Entities", opt => opt.Ignore());
                cfg.CreateMap<UserPublicMetricsV2, Public>();
                cfg.CreateMap<AfterExecutingQueryEventArgs, ResponseData>().ConvertUsing(new ResponseDataConverter());
            });
            return mapperConfig.CreateMapper();
        }        
    }

    internal class DateTimeConverter : ITypeConverter<DateTimeOffset, DateTime>
    {
        public DateTime Convert(DateTimeOffset source, DateTime destination, ResolutionContext context)
        {
            return source.DateTime;
        }
    }

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
            destination.Server = source.HttpHeaders["Server"].First();
            
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
