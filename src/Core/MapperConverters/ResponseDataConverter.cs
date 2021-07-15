using System;
using System.Linq;
using AutoMapper;
using BluebirdPS.Models;
using Tweetinvi.Events;

namespace BluebirdPS.Core.MapperConverters
{
    internal class ResponseDataConverter : ITypeConverter<AfterExecutingQueryEventArgs, ResponseData>
    {
        public ResponseData Convert(AfterExecutingQueryEventArgs source, ResponseData destination, ResolutionContext context)
        {
            destination = new ResponseData();

            Uri endpointUri = new(source.Url);

            destination.Timestamp = source.CompletedDateTime.DateTime;
            destination.HttpMethod = (HttpMethod)source.TwitterQuery.HttpMethod;
            destination.Uri = endpointUri;
            destination.Endpoint = endpointUri.AbsolutePath;
            destination.QueryString = endpointUri.Query;
            //destination.Body = null;
            //destination.Form = null;

            string authzHeader = source.TwitterQuery.AuthorizationHeader;
            if (authzHeader.Contains("OAuth"))
            {
                destination.OAuthVersion = OAuthVersion.OAuth1a;
            }
            else if (authzHeader.Contains("Bearer"))
            {
                destination.OAuthVersion = OAuthVersion.OAuth2Bearer;
            }
            else if (authzHeader.Contains("Basic"))
            {
                destination.OAuthVersion = OAuthVersion.Basic;
            }

            //destination.Status = null;
            if (source.HttpHeaders.ContainsKey("Server"))
            {
                destination.Server = source.HttpHeaders["Server"].First();
            }

            if (source.HttpHeaders.ContainsKey("x-response-time"))
            {
                destination.ResponseTime = int.Parse(source.HttpHeaders["x-response-time"].First());
            }

            if (source.QueryRateLimit != null)
            {
                destination.RateLimit = source.QueryRateLimit.Limit;
                destination.RateLimitReset = source.QueryRateLimit.ResetDateTime;
            }

            destination.HeaderResponse = source.HttpHeaders;
            destination.ApiVersion = endpointUri.Segments[1].Trim('/');
            destination.ApiResponse = source.HttpContent;

            return destination;
        }
    }
}
