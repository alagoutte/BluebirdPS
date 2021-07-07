using BluebirdPS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Tweetinvi.Models;

namespace BluebirdPS.Core
{
    internal class Credentials
    {
        private static OAuth oauth;

        private static OAuth Create()
        {
            if (EnvVarContainsCredentials())
            {
                oauth = GetCredentialsFromEnvVar();
            }
            else
            {
                string credentialsFile = ReadCredentialsFromFile();
                oauth = GetCredentialsFromFile(credentialsFile);
            }

            return oauth;
        }

        public static OAuth GetOrCreateInstance(bool newInstance = false)
        {

            return newInstance ? Create() : oauth;

        }

        internal static TwitterCredentials GetTwitterCredentials()
        {
            return new TwitterCredentials(
                    oauth.ApiKey,
                    oauth.ApiSecret,
                    oauth.AccessToken,
                    oauth.AccessTokenSecret
                );

        }
        internal static bool EnvVarContainsCredentials()
        {
            string apiKey = Environment.GetEnvironmentVariable("BLUEBIRDPS_API_KEY");
            string apiSecret = Environment.GetEnvironmentVariable("BLUEBIRDPS_API_SECRET");
            string accessToken = Environment.GetEnvironmentVariable("BLUEBIRDPS_ACCESS_TOKEN");
            string accessTokenSecret = Environment.GetEnvironmentVariable("BLUEBIRDPS_ACCESS_TOKEN_SECRET");

            if (string.IsNullOrEmpty(apiKey)) { return false; }
            if (string.IsNullOrEmpty(apiSecret)) { return false; }
            if (string.IsNullOrEmpty(accessToken)) { return false; }
            if (string.IsNullOrEmpty(accessTokenSecret)) { return false; }

            return true;
        }

        internal static Dictionary<string, string> GetCredentialsFromEnvVars()
        {
            if (EnvVarContainsCredentials())
            {
                Dictionary<string, string> credentialDictionary = new Dictionary<string, string>()
                {
                    { "BLUEBIRDPS_API_KEY", Environment.GetEnvironmentVariable("BLUEBIRDPS_API_KEY") },
                    { "BLUEBIRDPS_API_SECRET", Environment.GetEnvironmentVariable("BLUEBIRDPS_API_SECRET") },
                    { "BLUEBIRDPS_ACCESS_TOKEN", Environment.GetEnvironmentVariable("BLUEBIRDPS_ACCESS_TOKEN") },
                    { "BLUEBIRDPS_ACCESS_TOKEN_SECRET", Environment.GetEnvironmentVariable("BLUEBIRDPS_ACCESS_TOKEN_SECRET") }
                };

                string bearerToken = (Environment.GetEnvironmentVariable("BLUEBIRDPS_BEARER_TOKEN"));
                if (bearerToken != null)
                {
                    credentialDictionary.Add("BLUEBIRDPS_BEARER_TOKEN", bearerToken);
                }

                return credentialDictionary;
            }
            else
            {
                return null;
            }
        }

        internal static OAuth GetCredentialsFromEnvVar()
        {
            OAuth credentials = new OAuth();

            Dictionary<string, string> envCredentials = GetCredentialsFromEnvVars();

            credentials.ApiKey = envCredentials["BLUEBIRDPS_API_KEY"];
            credentials.ApiSecret = envCredentials["BLUEBIRDPS_API_SECRET"];
            credentials.AccessToken = envCredentials["BLUEBIRDPS_ACCESS_TOKEN"];
            credentials.AccessTokenSecret = envCredentials["BLUEBIRDPS_ACCESS_TOKEN_SECRET"];

            if (envCredentials["BLUEBIRDPS_BEARER_TOKEN"] != null)
            {
                credentials.BearerToken = envCredentials["BLUEBIRDPS_BEARER_TOKEN"];
            }

            return credentials;
        }

        internal static string ReadCredentialsFromFile()
        {
            return PSCommands.GetContents(Config.credentialsPath);
        }

        internal static OAuth GetCredentialsFromFile(string input)
        {
            SecureString credentialsFromDisk = PSCommands.ConvertToSecureString(input);
            string stringCredentials = PSCommands.ConvertFromSecureString(credentialsFromDisk, true);

            return JsonConverters.ConvertFromJson<OAuth>(stringCredentials);
        }

        internal static string ConvertCredentialsForFile()
        {
            string oAuthString = JsonConverters.ConvertToJson(oauth);
            SecureString secureOAuth = PSCommands.ConvertToSecureString(oAuthString, true);
            return PSCommands.ConvertFromSecureString(secureOAuth);
        }

        internal static void SaveCredentialsToFile()
        {
            File.WriteAllText(Config.credentialsPath, ConvertCredentialsForFile());
        }
    }
}
