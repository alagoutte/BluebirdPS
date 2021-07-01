using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.IO;
using System.Collections;

namespace BluebirdPS
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
