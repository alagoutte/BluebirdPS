using System;
using Tweetinvi;
using Tweetinvi.Events;
using BluebirdPS.Models;
using AutoMapper;
using Tweetinvi.Core.Exceptions;
using System.Management.Automation;
using System.Linq;

namespace BluebirdPS.Core
{
    internal class BluebirdPSClient
    {
        static BluebirdPSClient() {

            TweetinviEvents.BeforeWaitingForRequestRateLimits += BeforeWaitingForRequestRateLimits;
            TweetinviEvents.WaitingForRateLimit += WaitingForRateLimit;
            TweetinviEvents.BeforeExecutingRequest += BeforeExecutingRequest;
            TweetinviEvents.AfterExecutingRequest += AfterExecutingRequest;
            TweetinviEvents.OnTwitterException += OnTwitterException;
        }

        private static TwitterClient client;
        public static TwitterClient GetOrCreateInstance() => client ??= Create();
        private static TwitterClient Create()
        {
            client = new TwitterClient(
                Environment.GetEnvironmentVariable("BLUEBIRDPS_API_KEY"),
                Environment.GetEnvironmentVariable("BLUEBIRDPS_API_SECRET"),
                Environment.GetEnvironmentVariable("BLUEBIRDPS_ACCESS_TOKEN"),
                Environment.GetEnvironmentVariable("BLUEBIRDPS_ACCESS_TOKEN_SECRET")
            );

            client.Config.RateLimitTrackerMode = RateLimitTrackerMode.TrackOnly;
            TweetinviEvents.SubscribeToClientEvents(client);

            return client;
        }

        private static void AfterExecutingRequest(object sender, AfterExecutingQueryEventArgs args)
        {
            if (args.Url != "https://api.twitter.com/1.1/application/rate_limit_status.json")
            {
                IMapper mapper = BluebirdPSMapper.GetOrCreateInstance();
                ResponseData history = mapper.Map<ResponseData>(args);
                Metadata.History.Add(history);

                Metadata.AfterExecutingRequest.Add(args);
            }
        }

        private static void OnTwitterException(object sender, ITwitterException e)
        {
            Metadata.OnTwitterException.Add(e);
        }

        private static void BeforeExecutingRequest(object sender, BeforeExecutingRequestEventArgs e)
        {
            Metadata.BeforeExecutingRequest.Add(e);
        }

        private static void WaitingForRateLimit(object sender, WaitingForRateLimitEventArgs e)
        {
            Metadata.WaitingForRateLimit.Add(e);
        }

        private static void BeforeWaitingForRequestRateLimits(object sender, BeforeExecutingRequestEventArgs e)
        {
            Metadata.BeforeWaitingForRequestRateLimits.Add(e);
        }
    }
}
