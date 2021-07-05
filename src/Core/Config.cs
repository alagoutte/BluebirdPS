﻿using BluebirdPS.Models;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using Tweetinvi.Core.Exceptions;
using Tweetinvi.Events;
using Tweetinvi.Models;

namespace BluebirdPS.Core
{
    internal class Config
    {
        internal static readonly string configurationPath = Path.Join(Helpers.GetFileSavePath(), "Configuration.json");
        internal static readonly string credentialsPath = Path.Join(Helpers.GetFileSavePath(), "twittercred.sav");

        private static Configuration configuration;

        private static Configuration Create()
        {
            // create .BluebirdPS folder
            if (!Directory.Exists(Helpers.GetFileSavePath()))
            {
                _ = Directory.CreateDirectory(Helpers.GetFileSavePath());
            }

            // create config file
            if (!File.Exists(configurationPath))
            {
                _ = File.Create(configurationPath);
                configuration = new Configuration()
                {
                    RateLimitAction = RateLimitAction.Warning,
                    RateLimitThreshold = 5,
                    ConfigurationPath = configurationPath,
                    CredentialsPath = credentialsPath,
                    OutputType = OutputType.BluebirdPS
                };
            }
            else
            {
                ImportConfiguration();
            }

            return configuration;
        }

        public static Configuration GetOrCreateInstance() => configuration ??= Create();

        internal static void ImportConfiguration()
        {
            configuration = JsonConverters.ConvertFromJson<Configuration>(File.ReadAllText(configurationPath));
        }

        internal static void ExportConfiguration()
        {
            string config = JsonConverters.ConvertToJson(configuration);
            File.WriteAllTextAsync(configuration.ConfigurationPath, config);
        }

        internal static TwitterCredentials GetTwitterCredentials()
        {
            OAuth bluebirdPSOAuth = Credentials.GetOrCreateInstance();
            if (bluebirdPSOAuth != null)
            {
                return new TwitterCredentials(
                            bluebirdPSOAuth.ApiKey,
                            bluebirdPSOAuth.ApiSecret,
                            bluebirdPSOAuth.AccessToken,
                            bluebirdPSOAuth.AccessTokenSecret
                        );
            }
            return null;
        }

        internal static InvocationInfo InvocationInfo { get; set; }

        //internal static List<BeforeExecutingRequestEventArgs> BeforeWaitingForRequestRateLimits { get; set; } = new List<BeforeExecutingRequestEventArgs>();
        //internal static List<WaitingForRateLimitEventArgs> WaitingForRateLimit { get; set; } = new List<WaitingForRateLimitEventArgs>();
        //internal static List<BeforeExecutingRequestEventArgs> BeforeExecutingRequest { get; set; } = new List<BeforeExecutingRequestEventArgs>();
        internal static List<AfterExecutingQueryEventArgs> AfterExecutingRequest { get; set; } = new List<AfterExecutingQueryEventArgs>();
        internal static List<ITwitterException> OnTwitterException { get; set; } = new List<ITwitterException>();
    }
}
