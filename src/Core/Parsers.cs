using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using Newtonsoft.Json;
using BluebirdPS.Models;
using BluebirdPS.Models.APIV1;
using BluebirdPS.Models.APIV2;
using BluebirdPS.Models.Exceptions;
using System.Text.RegularExpressions;

namespace BluebirdPS.Core
{
    public class Parsers
    {        
        public static string GetErrorCategory(string errorType)
        {
            Hashtable _errorCategoryV2 = new Hashtable
            {
                { "about:blank", "NotSpecified" },
                { "https://api.twitter.com/2/problems/client-disconnected", "ConnectionError" },
                { "https://api.twitter.com/2/problems/client-forbidden", "PermissionDenied"},
                { "https://api.twitter.com/2/problems/disallowed-resource", "PermissionDenied"},
                { "https://api.twitter.com/2/problems/duplicate-rules", "InvalidOperation" },                
                { "https://api.twitter.com/2/problems/invalid-request", "InvalidArgument" },
                { "https://api.twitter.com/2/problems/invalid-rules", "InvalidArgument" },
                { "https://api.twitter.com/2/problems/not-authorized-for-field", "PermissionDenied" },
                { "https://api.twitter.com/2/problems/not-authorized-for-resource", "PermissionDenied"},
                { "https://api.twitter.com/2/problems/operational-disconnect", "ResourceUnavailable"},
                { "https://api.twitter.com/2/problems/resource-not-found", "ObjectNotFound" },
                { "https://api.twitter.com/2/problems/resource-unavailable", "ResourceUnavailable"},
                { "https://api.twitter.com/2/problems/rule-cap", "QuotaExceeded" },
                { "https://api.twitter.com/2/problems/streaming-connection", "ConnectionError" },
                { "https://api.twitter.com/2/problems/unsupported-authentication", "AuthenticationError" },
                { "https://api.twitter.com/2/problems/usage-capped", "QuotaExceeded" }                
            };

            if (_errorCategoryV2.ContainsKey(errorType))
            {
                return _errorCategoryV2[errorType].ToString();
            }
            else
            {
                return "NotSpecified";
            }
        }

        public static string GetErrorCategory(int statusCode, int errorCode)
        {
            Hashtable _errorCategoryV1= new Hashtable()
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

            switch (statusCode)
            {
                case 406:
                    return "InvalidData";
                case 415:
                    return "LimitsExceeded";
                case 420:
                    return "QuotaExceeded";
                case 422:
                    return errorCode == 404 ? "InvalidOperation" : "InvalidArgument";
                default:
                    foreach (KeyValuePair<int, Hashtable> kvp in _errorCategoryV1)
                    {
                        if (kvp.Key == statusCode)
                        {
                            Hashtable nested = (Hashtable)_errorCategoryV1[kvp.Key];
                            if (nested.ContainsKey(errorCode))
                            {
                                return nested[errorCode].ToString();
                            }
                        }
                    }
                    break;
            }

            return "NotSpecified";
        }
        
        internal static Exception GetTwitterAPIException(string exceptionType, string errorMessage)
        {
            return exceptionType switch
            {
                "AuthenticationException" => new BluebirdPSAuthenticationException(errorMessage),
                "InvalidOperationException" => new BluebirdPSInvalidOperationException(errorMessage),
                "InvalidArgumentException" => new BluebirdPSInvalidArgumentException(errorMessage),
                "LimitsExceededException" => new BluebirdPSLimitsExceededException(errorMessage),
                "ResourceViolationException" => new BluebirdPSResourceViolationException(errorMessage),
                "ResourceNotFoundException" => new BluebirdPSResourceNotFoundException(errorMessage),
                "SecurityException" => new BluebirdPSSecurityException(errorMessage),
                "ConnectionException" => new BluebirdPSConnectionException(errorMessage),
                "UnspecifiedException" => new BluebirdPSUnspecifiedException(errorMessage),
                _ => new BluebirdPSUnspecifiedException(errorMessage),
            };
        }

        internal static List<object> ParseApiV2Response(dynamic input)
        {
            List<object> twitterResponse = new List<object>();

            if (Helpers.HasProperty(input, "data"))
            {
                if (input.data is Array)
                {
                    foreach (dynamic twitterObject in input.data)
                    {
                        if (Helpers.HasProperty(twitterObject, "username"))
                        {
                            twitterResponse.Add(new User(twitterObject));
                        }
                        else if (Helpers.HasProperty(twitterObject, "text"))
                        {
                            twitterResponse.Add(new Tweet(twitterObject));
                        }
                        else if (Helpers.HasProperty(twitterObject, "tweet_count"))
                        {
                            twitterResponse.Add(new TweetCount(twitterObject));
                        }
                    }
                }
                else
                {
                    if (Helpers.HasProperty(input.data, "following"))
                    {
                        GetUpdateFriendshipStatus(input);
                    }
                    else if (Helpers.HasProperty(input.data, "blocking"))
                    {
                        GetUserBlockStatus(input);
                    }
                    else if (Helpers.HasProperty(input.data, "liked"))
                    {
                        GetTweetLikeStatus(input);
                    }
                    else if (Helpers.HasProperty(input.data, "username"))
                    {
                        twitterResponse.Add(new User(input.data));
                    }
                    else if (Helpers.HasProperty(input.data, "text"))
                    {
                        twitterResponse.Add(new Tweet(input.data));
                    }
                }
            }
            // parse includes
            if (Helpers.HasProperty(input.includes, "tweets"))
            {
                foreach (dynamic tweet in input.includes.tweets)
                {
                    twitterResponse.Add(new Tweet(tweet));
                }
            }
            if (Helpers.HasProperty(input.includes, "users"))
            {
                foreach (dynamic user in input.includes.users)
                {
                    twitterResponse.Add(new User(user));
                }
            }
            if (Helpers.HasProperty(input.includes, "media"))
            {
                foreach (dynamic thisMedia in input.includes.media)
                {
                    twitterResponse.Add(new Models.APIV2.Media(thisMedia));
                }
            }
            if (Helpers.HasProperty(input.includes, "polls"))
            {
                foreach (dynamic poll in input.includes.polls)
                {
                    twitterResponse.Add(new Poll(poll));
                }
            }

            return twitterResponse;
        }
        
        internal static List<object> ParseApiV1Response(dynamic input)
        {
            List<object> twitterResponse = new List<object>();

            if (input is Array)
            {
                foreach (dynamic twitterObject in input)
                {
                    if (Helpers.HasProperty(twitterObject, "lists") || Helpers.HasProperty(twitterObject, "member_count"))
                    {
                        twitterResponse.Add(new List(twitterObject));
                    }
                    else if (Helpers.HasProperty(twitterObject, "connections"))
                    {
                        twitterResponse.Add(new FriendshipConnections(twitterObject));
                    }
                    else if (Helpers.HasProperty(twitterObject, "screen_name"))
                    {
                        twitterResponse.Add(new User(twitterObject));
                    }
                    else if (Helpers.HasProperty(twitterObject, "query"))
                    {
                        twitterResponse.Add(new SavedSearch(twitterObject));
                    }
                    else if (Helpers.HasProperty(twitterObject, "in_reply_to_user_id_str"))
                    {
                        twitterResponse.Add(new Tweet(twitterObject));
                    }
                    else
                    {
                        twitterResponse.Add(twitterObject);
                    }
                }
            }
            else
            {
                if (Helpers.HasProperty(input, "lists"))
                {
                    foreach (dynamic twitterObject in input.lists)
                    {
                        twitterResponse.Add(new List(twitterObject));
                    }
                }
                else if (Helpers.HasProperty(input, "discoverable_by_email"))
                {
                    twitterResponse.Add(new AccountSettings(input));
                }
                else if (Helpers.HasProperty(input, "relationship"))
                {
                    twitterResponse.Add(new Relationship(input.relationship));
                }
                else if (Helpers.HasProperty(input, "member_count"))
                {
                    twitterResponse.Add(new List(input));
                }
                else if (Helpers.HasProperty(input, "screen_name"))
                {
                    twitterResponse.Add(new User(input));
                }
                else if (Helpers.HasProperty(input, "query"))
                {
                    twitterResponse.Add(new SavedSearch(input));
                }
                else if (Helpers.HasProperty(input, "ids"))
                {
                    twitterResponse.AddRange(input.ids);
                }
                else if (Helpers.HasProperty(input, "events") || Helpers.HasProperty(input, "event"))
                {
                    dynamic directMessages = null;
                    if (Helpers.HasProperty(input, "events"))
                    {
                        directMessages = input.events;
                    }
                    else if (Helpers.HasProperty(input, "event"))
                    {
                        directMessages = input.@event;
                    }
                    if (directMessages is IEnumerable)
                    {
                        foreach (dynamic message in directMessages)
                        {
                            twitterResponse.Add(new DirectMessage(message));
                        }
                    }
                    else
                    {
                        twitterResponse.Add(new DirectMessage(directMessages));
                    }

                }
                else if (Helpers.HasProperty(input, "users"))
                {
                    foreach (dynamic user in input.users)
                    {
                        twitterResponse.Add(new User(user));
                    }
                }
                //else if (Helpers.HasProperty(input, "text"))
                //{
                //    twitterResponse.Add(new Tweet(input));
                //}
                else
                {
                    if (!Helpers.HasProperty(input, "errors"))
                    {
                        twitterResponse.Add(input);
                    }
                }
            }

            return twitterResponse;
        }

        internal static string ConvertToJson(object input)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            
            //return JsonSerializer.Serialize(input, options);
            return JsonConvert.SerializeObject(input, Formatting.Indented);
        }

        internal static dynamic ConvertFromJson(string input)
        {
            //return JsonSerializer.Deserialize<dynamic>(input);
            return JsonConvert.DeserializeObject(input);
        }

        internal static T ConvertFromJson<T>(string input)
        {
            //return JsonSerializer.Deserialize<T>(input);
            return JsonConvert.DeserializeObject<T>(input);
        }

        internal static bool IsJson(string text)
        {
            try
            {
                _ = JsonConvert.DeserializeObject(text);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetUpdateFriendshipStatus(ResponseData response)
        {
            string following = response.Command == "Add-TwitterFriend" ? GetUserFromBody(response) : GetUserFromSegment(response);
            bool isPending = Helpers.HasProperty(response.ApiResponse.data, "pending_follow") ? (bool)response.ApiResponse.data.pending_follow : false;
            string pending = isPending ? $" There is a pending follow: {response.ApiResponse.data.pending_follow}" : null;
            return $"Following user {following}: {response.ApiResponse.data.following}.{pending}";
        }

        public static string GetUserBlockStatus(ResponseData response)
        {
            string blocking = response.HttpMethod == HttpMethod.POST ? GetUserFromBody(response) : GetUserFromSegment(response);
            return $"Blocking user {blocking}: {response.ApiResponse.data.blocking}";
        }

        public static string GetTweetLikeStatus(ResponseData response)
        {
            string liked = response.HttpMethod == HttpMethod.POST ? GetUserFromBody(response) : GetUserFromSegment(response);
            return $"Likes tweet {liked}: {response.ApiResponse.data.liked}";
        }

        //public static string GetUserMuteStatus(ResponseData response)
        //{
        //    // return nothing as the returned v1.1 user 'muting' property may not have been updated
        //    // an error will still be returned if an attempt to unmute a user that hasn't been muted
        //    return $"Muting user {response.ApiResponse.screen_name}: {response.ApiResponse.muting}";
        //}

        private static string GetUserFromBody(ResponseData response)
        {
            Regex r = new Regex(@"(?<target>\d+)");
            Match m = r.Match(response.Body);
            return m.Groups["target"].Value;
        }

        private static string GetUserFromSegment(ResponseData response)
        {
            return response.Uri.Segments[5].Replace("/", null);
        }
    }
}
