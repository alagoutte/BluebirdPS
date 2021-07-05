using BluebirdPS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace BluebirdPS.Core
{
    internal class Credentials
    {
        private static OAuth oauth;

        private static OAuth Create()
        {
            return new OAuth();
        }

        public static OAuth GetOrCreateInstance() => oauth ??= Create();

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

        internal static OAuth ImportCredentialsFromFile()
        {
            using PowerShell pwsh = PowerShell.Create(RunspaceMode.CurrentRunspace);
            string credentialsFromDisk = pwsh.AddCommand("Get-Content")
                .AddParameter("Path", Config.credentialsPath)
                .AddCommand("ConvertTo-SecureString")
                .AddCommand("ConvertFrom-SecureString")
                .AddParameter("AsPlainText", true)
                .Invoke<string>().ToList().First();

            return JsonConverters.ConvertFromJson<OAuth>(credentialsFromDisk);
        }

        internal static void ExportCredentialsToFile()
        {
            string oAuthString = JsonConverters.ConvertToJson(oauth);
            using PowerShell pwsh = PowerShell.Create(RunspaceMode.CurrentRunspace);
            string secureString = pwsh.AddCommand("ConvertTo-SecureString")
                .AddParameter("String", oAuthString)
                .AddParameter("AsPlainText", true)
                .AddCommand("ConvertFrom-SecureString")
                .Invoke<string>().ToList().First();

            File.WriteAllText(Config.credentialsPath, secureString);
        }

        internal static void SaveCredentialsToFile()
        {

        }

        internal static void SaveCredentialsToEnvironmentVariables()
        {

        }
    }
}
