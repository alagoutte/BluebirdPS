using AutoMapper;
using BluebirdPS.Models;
using BluebirdPS.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using Tweetinvi;
using Tweetinvi.Core.Exceptions;
using Tweetinvi.Events;
using Tweetinvi.Models;

namespace BluebirdPS.Core
{
    internal class Client
    {
        private static OAuth oauth = Credentials.GetOrCreateInstance();
        private static Configuration configuration = Config.GetOrCreateInstance();

        static Client()
        {
            TweetinviEvents.AfterExecutingRequest += AfterExecutingRequest;
            TweetinviEvents.OnTwitterException += OnTwitterException;
        }

        private static TwitterClient client;
        public static TwitterClient GetOrCreateInstance() => client ??= Create();

        private static TwitterClient Create()
        {
            if (Credentials.HasCredentialsInEnvVars())
            {
                oauth = Credentials.ReadCredentialsFromEnvVars();
            }
            else
            {
                oauth = Credentials.ReadCredentialsFromFile();
            }

            if (oauth.IsNull())
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine($"Credentials were not found in environment variables(BLUEBIRDPS_ *) or in { Config.credentialsPath}");
                message.AppendLine("Please use the Set-TwitterAuthentication command to update the required API keys and secrets.");
                message.AppendLine("For more information, see conceptual help topic about_BluebirdPS_Credentials");

                throw new BluebirdPSNullCredentialsException(message.ToString());
            }

            client = new TwitterClient(new TwitterCredentials(
                    oauth.ApiKey,
                    oauth.ApiSecret,
                    oauth.AccessToken,
                    oauth.AccessTokenSecret
                    ));

            // add any Configuration values here
            client.Config.RateLimitTrackerMode = RateLimitTrackerMode.TrackOnly;

            TweetinviEvents.SubscribeToClientEvents(client);

            return client;
        }


        private static void AfterExecutingRequest(object sender, AfterExecutingQueryEventArgs args)
        {
            if (args.Exception != null)
            {
                throw new Exception(args.Exception.Message);
            }

            IMapper mapper = Mapper.GetOrCreateInstance();
            List<ResponseData> history = History.GetOrCreateInstance();

            ResponseData historyRecord = mapper.Map<ResponseData>(args);
            history.Add(historyRecord);

        }

        private static void OnTwitterException(object sender, ITwitterException e)
        {
            System.Console.WriteLine(e.Content);
        }

    }
}
