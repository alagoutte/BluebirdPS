using BluebirdPS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security;
using Tweetinvi.Core.Exceptions;
using Tweetinvi.Events;
using Tweetinvi.Models;

namespace BluebirdPS.Core
{
    internal class Config
    {
        private static readonly string configurationPath = Path.Join(Helpers.GetFileSavePath(), "Configuration.json");
        private static readonly string credentialsPath = Path.Join(Helpers.GetFileSavePath(), "twittercred.sav");

        private static Configuration configuration;
        //private static Credentials credentials;
        public static Configuration GetOrCreateInstance() => configuration ??= Create();

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

        internal static void ImportConfiguration()
        {
            configuration = Converters.ConvertFromJson<Configuration>(File.ReadAllText(configurationPath));                         
        }

        internal static void ExportConfiguration()
        {
            string config = Converters.ConvertToJson(configuration);
            File.WriteAllTextAsync(configuration.ConfigurationPath, config);
        }

        internal static Dictionary<string, string> GetCredentialsFromEnvironmentVariables()
        {
            if ((Environment.GetEnvironmentVariable("BLUEBIRDPS_API_KEY") ??
                Environment.GetEnvironmentVariable("BLUEBIRDPS_API_SECRET") ??
                Environment.GetEnvironmentVariable("BLUEBIRDPS_ACCESS_TOKEN") ??
                Environment.GetEnvironmentVariable("BLUEBIRDPS_ACCESS_TOKEN_SECRET")) != null)
            {
                Dictionary<string, string> credentialDictionary = new Dictionary<string, string>()
                {
                    { "BLUEBIRDPS_API_KEY", Environment.GetEnvironmentVariable("BLUEBIRDPS_API_KEY") },
                    { "BLUEBIRDPS_API_SECRET", Environment.GetEnvironmentVariable("BLUEBIRDPS_API_SECRET") },
                    { "BLUEBIRDPS_ACCESS_TOKEN", Environment.GetEnvironmentVariable("BLUEBIRDPS_ACCESS_TOKEN") },
                    { "BLUEBIRDPS_ACCESS_TOKEN_SECRET", Environment.GetEnvironmentVariable("BLUEBIRDPS_ACCESS_TOKEN_SECRET") }
                };

                if (Environment.GetEnvironmentVariable("BLUEBIRDPS_ACCESS_TOKEN_SECRET") != null)
                {
                    credentialDictionary.Add("BLUEBIRDPS_BEARER_TOKEN", Environment.GetEnvironmentVariable("BLUEBIRDPS_API_KEY"));
                }

                return credentialDictionary;
            } else
            {
                return null;
            }
        }

        internal static Credentials GetCredentialsFromFile()
        {
            string credentialsOnDisk = File.ReadAllText(credentialsPath);

            using PowerShell pwsh = PowerShell.Create(RunspaceMode.CurrentRunspace);
            string credentialsString = pwsh.AddCommand("ConvertTo-SecureString")
                .AddParameter("String", credentialsOnDisk)
                .AddCommand("ConvertFrom-SecureString")
                .AddParameter("AsPlainText", true).Invoke()[0].ToString();

            Credentials credentials = Converters.ConvertFromJson<Credentials>(credentialsString);
            return credentials;
        }

        internal static void SaveCredentialsToFile()
        {

        }
        internal static void SaveCredentialsToEnvironmentVariables()
        {

        }

        internal static TwitterCredentials ImportCredentials()
        {
            Dictionary<string, string> envCredentials = GetCredentialsFromEnvironmentVariables();
            if (envCredentials != null)
            {
                return new TwitterCredentials(
                        envCredentials["BLUEBIRDPS_API_KEY"],
                        envCredentials["BLUEBIRDPS_API_SECRET"],
                        envCredentials["BLUEBIRDPS_ACCESS_TOKEN"],
                        envCredentials["BLUEBIRDPS_ACCESS_TOKEN_SECRET"]
                    );
            }
            else
            {
                Credentials diskCredentials = GetCredentialsFromFile();
                if (diskCredentials != null)
                {
                    return new TwitterCredentials(
                            diskCredentials.ApiKey,
                            diskCredentials.ApiSecret,
                            diskCredentials.AccessToken,
                            diskCredentials.AccessTokenSecret
                        );
                }
            }
            return null;
        }

        internal static void ExportCredentials()
        {

        }

        internal static string ConvertFromSecureString(SecureString input)
        {
            IntPtr strptr = Marshal.SecureStringToBSTR(input);
            string normalString = Marshal.PtrToStringBSTR(strptr);
            Marshal.ZeroFreeBSTR(strptr);
            return normalString;
        }

        internal static SecureString ConvertToSecureString(string input)
        {
            SecureString secureString = new SecureString();
            foreach (var item in input)
            {
                secureString.AppendChar(item);
            }
            return secureString;
        }

        internal static InvocationInfo InvocationInfo { get; set; }

        //internal static List<BeforeExecutingRequestEventArgs> BeforeWaitingForRequestRateLimits { get; set; } = new List<BeforeExecutingRequestEventArgs>();
        //internal static List<WaitingForRateLimitEventArgs> WaitingForRateLimit { get; set; } = new List<WaitingForRateLimitEventArgs>();
        //internal static List<BeforeExecutingRequestEventArgs> BeforeExecutingRequest { get; set; } = new List<BeforeExecutingRequestEventArgs>();
        internal static List<AfterExecutingQueryEventArgs> AfterExecutingRequest { get; set; } = new List<AfterExecutingQueryEventArgs>();
        internal static List<ITwitterException> OnTwitterException { get; set; } = new List<ITwitterException>();
    }
}
