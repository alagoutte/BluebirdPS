using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net;

namespace BluebirdPS.Models
{
    public class ResponseData
    {
        public string Command { get; set; }
        public DateTime Timestamp { get; set; }
        public InvocationInfo InvocationInfo { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public Uri Uri { get; set; }
        public string Endpoint { get; set; }
        public string QueryString { get; set; }
        public string Body { get; set; }
        public Hashtable Form { get; set; }
        public OAuthVersion OAuthVersion { get; set; }
        public HttpStatusCode Status { get; set; }
        public string Server { get; set; }
        public int? ResponseTime { get; set; }
        public int? RateLimit { get; set; }
        public int? RateLimitRemaining { get; set; }
        public DateTime RateLimitReset { get; set; }
        public Dictionary<string, IEnumerable<string>> HeaderResponse { get; set; }
        public string ApiVersion { get; set; }
        public dynamic ApiResponse { get; set; }

    }
}
