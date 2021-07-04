using System;
using System.Collections.Generic;
using System.Text;

namespace BluebirdPS.Models
{
    internal class Credentials
    {
        internal string ApiKey { get; set; }
        internal string ApiSecret { get; set; }
        internal string AccessToken { get; set; }
        internal string AccessTokenSecret { get; set; }
        internal string BearerToken { get; set; }

    }
}
