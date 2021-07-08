using BluebirdPS.Models;
using System;
using System.IO;
using System.Security;

namespace BluebirdPS.Core
{
    internal class Credentials
    {
        private static OAuth oauth;
        private static OAuth Create()
        {
            oauth = new OAuth();
            return oauth;
        }

        public static OAuth GetOrCreateInstance() => oauth ??= Create();

        internal static bool HasCredentialsInEnvVars()
        {
            string apiKey = Environment.GetEnvironmentVariable("BLUEBIRDPS_API_KEY");
            string apiSecret = Environment.GetEnvironmentVariable("BLUEBIRDPS_API_SECRET");
            string accessToken = Environment.GetEnvironmentVariable("BLUEBIRDPS_ACCESS_TOKEN");
            string accessTokenSecret = Environment.GetEnvironmentVariable("BLUEBIRDPS_ACCESS_TOKEN_SECRET");

            if (string.IsNullOrEmpty(apiKey) ||
                string.IsNullOrEmpty(apiSecret) ||
                string.IsNullOrEmpty(accessToken) ||
                string.IsNullOrEmpty(accessTokenSecret)
                )
            {
                return false;
            }
            return true;
        }
        internal static OAuth ReadCredentialsFromEnvVars()
        {
            OAuth credentials = new OAuth();
            if (HasCredentialsInEnvVars())
            {
                credentials.ApiKey = Environment.GetEnvironmentVariable("BLUEBIRDPS_API_KEY");
                credentials.ApiSecret = Environment.GetEnvironmentVariable("BLUEBIRDPS_API_SECRET");
                credentials.AccessToken = Environment.GetEnvironmentVariable("BLUEBIRDPS_ACCESS_TOKEN");
                credentials.AccessTokenSecret = Environment.GetEnvironmentVariable("BLUEBIRDPS_ACCESS_TOKEN_SECRET");

                if (Environment.GetEnvironmentVariable("BLUEBIRDPS_BEARER_TOKEN") != null)
                {
                    credentials.BearerToken = Environment.GetEnvironmentVariable("BLUEBIRDPS_BEARER_TOKEN");
                }

                oauth = credentials;
            }
            return oauth;
        }
        internal static OAuth ReadCredentialsFromFile()
        {
            string encryptedCredentials = PSCommands.GetContents(Config.credentialsPath);
            SecureString credentialsFromDisk = PSCommands.ConvertToSecureString(encryptedCredentials);
            string stringCredentials = PSCommands.ConvertFromSecureString(credentialsFromDisk, true);

            OAuth credentials = JsonConverters.ConvertFromJson<OAuth>(stringCredentials);
            oauth = credentials;
            return oauth;
        }
        internal static string ConvertCredentialsForFile(OAuth oauthCredentials)
        {
            string oAuthString = JsonConverters.ConvertToJson(oauthCredentials);
            SecureString secureOAuth = PSCommands.ConvertToSecureString(oAuthString, true);
            return PSCommands.ConvertFromSecureString(secureOAuth);
        }
        internal static void SaveCredentialsToFile(OAuth oauthCredentials)
        {
            File.WriteAllText(Config.credentialsPath, ConvertCredentialsForFile(oauthCredentials));
        }
    }
}
