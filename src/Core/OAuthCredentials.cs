using System;
using System.Collections.Generic;
using System.Text;

namespace BluebirdPS.Core
{
    internal class OAuthCredentials
    {
        internal string ApiKey { get; set; }
        internal string ApiSecret { get; set; }
        internal string AccessToken { get; set; }
        internal string AccessTokenSecret { get; set; }
        internal string BearerToken { get; set; }

        internal OAuthCredentials()
        {
            ApiKey = Environment.GetEnvironmentVariable("BLUEBIRDPS_API_KEY");
            ApiSecret = Environment.GetEnvironmentVariable("BLUEBIRDPS_API_SECRET");
            AccessToken = Environment.GetEnvironmentVariable("BLUEBIRDPS_ACCESS_TOKEN");
            AccessTokenSecret = Environment.GetEnvironmentVariable("BLUEBIRDPS_ACCESS_TOKEN_SECRET");
            BearerToken = Environment.GetEnvironmentVariable("BLUEBIRDPS_BEARER_TOKEN");
        }
    }

}
