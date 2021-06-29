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
        internal static Configuration Configuration { get; set; }
        internal static List<ResponseData> History { get; set; }
        internal static string ConfigSavePath { get; } = Path.Join(Helpers. GetFileSavePath(), "Configuration.Json");
        internal static string CredentialsSavePath { get; } = Path.Join(Helpers.GetFileSavePath(), "twittercred.sav");
        internal static Hashtable ErrorCategoryV2 { get; } = new Hashtable
        {
            { "about:blank", "NotSpecified" },
            { "https://api.twitter.com/2/problems/not-authorized-for-resource", "PermissionDenied"},
            { "https://api.twitter.com/2/problems/not-authorized-for-field", "PermissionDenied" },
            { "https://api.twitter.com/2/problems/invalid-request", "InvalidArgument" },
            { "https://api.twitter.com/2/problems/client-forbidden", "PermissionDenied"},
            { "https://api.twitter.com/2/problems/disallowed-resource", "PermissionDenied"},
            { "https://api.twitter.com/2/problems/unsupported-authentication", "AuthenticationError" },
            { "https://api.twitter.com/2/problems/usage-capped", "QuotaExceeded" },
            { "https://api.twitter.com/2/problems/streaming-connection", "ConnectionError" },
            { "https://api.twitter.com/2/problems/client-disconnected", "ConnectionError" },
            { "https://api.twitter.com/2/problems/operational-disconnect", "ResourceUnavailable"},
            { "https://api.twitter.com/2/problems/rule-cap", "QuotaExceeded" },
            { "https://api.twitter.com/2/problems/invalid-rules", "InvalidArgument" },
            { "https://api.twitter.com/2/problems/duplicate-rules", "InvalidOperation" },
            { "https://api.twitter.com/2/problems/resource-not-found", "ObjectNotFound" }
        };
        internal static Hashtable ErrorCategoryV1 { get; } = new Hashtable()
        {
            { 400, new Hashtable()
                {
                    { 324, "OperationStopped" },
                    { 325, "ObjectNotFound" },
                    { 323, "InvalidOperation" },
                    { 110, "InvalidOperation" },
                    { 215, "AuthenticationError" },
                    { 3, "InvalidArgument" },
                    { 7, "InvalidArgument" },
                    { 8, "InvalidArgument" },
                    { 44, "InvalidArgument" },
                    { 407, "ResourceUnavailable" }
                }
            },
            { 401, new Hashtable()
                {
                    { 417, "AuthenticationError" },
                    { 135, "AuthenticationError" },
                    { 32, "AuthenticationError" },
                    { 416, "AuthenticationError" }
                }
            },
            { 403, new Hashtable()
                {
                    { 326, "SecurityError" },
                    { 200, "InvalidOperation" },
                    { 272, "InvalidOperation" },
                    { 160, "InvalidOperation" },
                    { 203, "InvalidOperation" },
                    { 431, "InvalidOperation" },
                    { 386, "QuotaExceeded" },
                    { 205, "QuotaExceeded" },
                    { 226, "QuotaExceeded" },
                    { 327, "QuotaExceeded" },
                    { 99, "AuthenticationError" },
                    { 89, "AuthenticationError" },
                    { 195, "ConnectionError" },
                    { 92, "ConnectionError" },
                    { 354, "InvalidArgument" },
                    { 186, "InvalidArgument" },
                    { 38, "InvalidArgument" },
                    { 120, "InvalidArgument" },
                    { 163, "InvalidArgument" },
                    { 214, "PermissionDenied" },
                    { 220, "PermissionDenied" },
                    { 261, "PermissionDenied" },
                    { 187, "PermissionDenied" },
                    { 349, "PermissionDenied" },
                    { 385, "PermissionDenied" },
                    { 415, "PermissionDenied" },
                    { 271, "PermissionDenied" },
                    { 185, "PermissionDenied" },
                    { 36, "PermissionDenied" },
                    { 63, "PermissionDenied" },
                    { 64, "PermissionDenied" },
                    { 87, "PermissionDenied" },
                    { 179, "PermissionDenied" },
                    { 93, "PermissionDenied" },
                    { 433, "PermissionDenied" },
                    { 139, "PermissionDenied" },
                    { 150, "PermissionDenied" },
                    { 151, "PermissionDenied" },
                    { 161, "PermissionDenied" },
                    { 425, "PermissionDenied" }
                }
            },
            { 404, new Hashtable()
                {
                    { 34, "ObjectNotFound" },
                    { 108, "ObjectNotFound" },
                    { 109, "ObjectNotFound" },
                    { 422, "ObjectNotFound" },
                    { 421, "ObjectNotFound" },
                    { 13, "ObjectNotFound" },
                    { 17, "ObjectNotFound" },
                    { 144, "ObjectNotFound" },
                    { 50, "ObjectNotFound" },
                    { 25, "InvalidArgument" }
                }
            },
            { 409, new Hashtable()
                {
                    { 355, "InvalidOperation" }
                }
            },
            { 410, new Hashtable()
                {
                    { 68, "ConnectionError" },
                    { 251, "NotImplemented" }
                }
            },
            { 429, new Hashtable()
                {
                    { 88, "QuotaExceeded" }
                }
            },
            { 500, new Hashtable()
                {
                    { 131, "ResourceUnavailable" }
                }
            },
            { 503, new Hashtable()
                {
                    { 130, "ResourceBusy" }
                }
            }
        };
        internal static int LastStatusCode {get; set;}
        internal static object ResponseHeaders {get; set;}
    }

}
