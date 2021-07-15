using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using AutoMapper;
using BluebirdPS.Models;
using BluebirdPS.Models.Exceptions;
using Tweetinvi;
using Tweetinvi.Core.Exceptions;
using Tweetinvi.Events;
using Tweetinvi.Models;

namespace BluebirdPS.Core
{
    internal sealed class Client
    {
        public static string CommandName { get; set; }
        public static InvocationInfo InvocationInfo { get; set; }
        public static TwitterClient Create(OAuth oauth, PSCmdlet cmdlet)
        {
            CommandName = cmdlet.MyInvocation.MyCommand.Name;
            InvocationInfo = cmdlet.MyInvocation;

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
                StringBuilder message = new();
                message.AppendLine($"Credentials were not found in environment variables(BLUEBIRDPS_ *) or in { Config.credentialsPath}");
                message.AppendLine("Please use the Set-TwitterAuthentication command to update the required API keys and secrets.");
                message.AppendLine("For more information, see conceptual help topic about_BluebirdPS_Credentials");

                throw new BluebirdPSNullCredentialsException(message.ToString());
            }

            TwitterClient client = new(new TwitterCredentials(
                    oauth.ApiKey,
                    oauth.ApiSecret,
                    oauth.AccessToken,
                    oauth.AccessTokenSecret
                    ));

            // add any Configuration values here
            client.Config.RateLimitTrackerMode = RateLimitTrackerMode.TrackOnly;

            client.Events.AfterExecutingRequest += AfterExecutingRequest;
            client.Events.OnTwitterException += OnTwitterException;

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
            historyRecord.InvocationInfo = InvocationInfo;
            historyRecord.Command = CommandName;

            history.Add(historyRecord);
        }

        private static void OnTwitterException(object sender, ITwitterException e)
        {
            System.Console.WriteLine(e.Content);
        }
    }
}
