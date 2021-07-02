using BluebirdPS.Models;
using System.Collections.Generic;

namespace BluebirdPS.Core
{
    internal class Metadata
    {
        internal static OAuthCredentials OAuth { get; set; } = new OAuthCredentials();
        internal static Configuration Configuration { get; set; } = new Configuration();
        internal static List<ResponseData> History { get; set; } = new List<ResponseData>();
        internal static int LastStatusCode {get; set;}
        internal static object LastResponseHeaders {get; set;}
    }

}
