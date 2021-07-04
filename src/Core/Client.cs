using AutoMapper;
using BluebirdPS.Models;
using System;
using System.Collections.Generic;
using Tweetinvi;
using Tweetinvi.Core.Exceptions;
using Tweetinvi.Events;
using Tweetinvi.Models;
using BluebirdPS.Core;
using Tweetinvi.Exceptions;

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
        public static TwitterClient GetOrCreateInstance() => client ??= Create();
        private static TwitterClient Create()
        {
            TwitterCredentials credentials = Config.ImportCredentials();
            
            if (credentials != null)
            {
                client = new TwitterClient(credentials);

                client.Config.RateLimitTrackerMode = RateLimitTrackerMode.TrackOnly;
                TweetinviEvents.SubscribeToClientEvents(client);
                return client;
            }

            throw new TwitterNullCredentialsException();
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
