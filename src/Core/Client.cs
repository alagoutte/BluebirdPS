using AutoMapper;
using BluebirdPS.Models;
using System.Collections.Generic;
using Tweetinvi;
using Tweetinvi.Core.Exceptions;
using Tweetinvi.Events;
using Tweetinvi.Models;

namespace BluebirdPS.Core
{
    internal class Client
    {
        static Client()
        {

            //TweetinviEvents.BeforeWaitingForRequestRateLimits += BeforeWaitingForRequestRateLimits;
            //TweetinviEvents.WaitingForRateLimit += WaitingForRateLimit;
            //TweetinviEvents.BeforeExecutingRequest += BeforeExecutingRequest;
            TweetinviEvents.AfterExecutingRequest += AfterExecutingRequest;
            TweetinviEvents.OnTwitterException += OnTwitterException;
        }

        private static TwitterClient client;
        public static TwitterClient GetOrCreateInstance(bool newInstance = false)
        {

            return newInstance ? Create() : client;

        }

        private static TwitterClient Create()
        {
            _ = Credentials.GetOrCreateInstance(true);

            TwitterCredentials credentials = Credentials.GetTwitterCredentials();

            client = new TwitterClient(credentials);

            client.Config.RateLimitTrackerMode = RateLimitTrackerMode.TrackOnly;
            TweetinviEvents.SubscribeToClientEvents(client);
            
            return client;
        }

        private static List<ResponseData> history = History.GetOrCreateInstance();
        private static void AfterExecutingRequest(object sender, AfterExecutingQueryEventArgs args)
        {
            if (args.Url != "https://api.twitter.com/1.1/application/rate_limit_status.json")
            {
                IMapper mapper = Mapper.GetOrCreateInstance();
                ResponseData historyRecord = mapper.Map<ResponseData>(args);
                history.Add(historyRecord);

                Config.AfterExecutingRequest.Add(args);
            }
        }

        private static void OnTwitterException(object sender, ITwitterException e)
        {
            Config.OnTwitterException.Add(e);
        }

        //private static void BeforeExecutingRequest(object sender, BeforeExecutingRequestEventArgs e)
        //{
        //    Config.BeforeExecutingRequest.Add(e);
        //}

        //private static void WaitingForRateLimit(object sender, WaitingForRateLimitEventArgs e)
        //{
        //    Config.WaitingForRateLimit.Add(e);
        //}

        //private static void BeforeWaitingForRequestRateLimits(object sender, BeforeExecutingRequestEventArgs e)
        //{
        //    Config.BeforeWaitingForRequestRateLimits.Add(e);
        //}
    }
}
