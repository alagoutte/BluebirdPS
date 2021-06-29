using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BluebirdPS;
using BluebirdPS.APIV1;
using BluebirdPS.APIV2.Objects;
using BluebirdPS.APIV2.TweetInfo;
using BluebirdPS.APIV2.UserInfo;

namespace BluebirdPS
{
    public class Parsers
    {
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

        public static List<object> ParseApiV2Response(dynamic input)
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
                    if (Helpers.HasProperty(input.data, "username"))
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
                    twitterResponse.Add(new Media(thisMedia));
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
        public static List<object> ParseApiV1Response(dynamic input)
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

    }
}
