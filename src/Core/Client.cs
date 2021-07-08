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
            TweetinviEvents.AfterExecutingRequest += AfterExecutingRequest;
            TweetinviEvents.OnTwitterException += OnTwitterException;
        }

        private static TwitterClient client;
        public static TwitterClient GetOrCreateInstance(TwitterCredentials twitterCredentials, Configuration configuration) => client ??= Create(twitterCredentials, configuration);

        private static TwitterClient Create(TwitterCredentials twitterCredentials, Configuration configuration)
        {
            client = new TwitterClient(twitterCredentials);

            // add any Configuration values here
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

    }
}
