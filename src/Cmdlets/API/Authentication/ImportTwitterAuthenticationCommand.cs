using BluebirdPS.Core;
using BluebirdPS.Models;
using System.Collections.Generic;
using System.Management.Automation;


namespace BluebirdPS.Cmdlets.Authentication
{
    [Cmdlet(VerbsData.Import, "TwitterAuthentication")]
    public class ImportTwitterAuthenticationCommand : PSCmdlet
    {
        OAuth oAuthCredentals = Credentials.GetOrCreateInstance();
        protected override void ProcessRecord()
        {
            if (Credentials.EnvVarContainsCredentials())
            {
                WriteVerbose("Importing credentials from environment variables.");

                Dictionary<string, string> envCredentials = Credentials.GetCredentialsFromEnvVars();
                oAuthCredentals = new OAuth()
                {
                    ApiKey = envCredentials["BLUEBIRDPS_API_KEY"],
                    ApiSecret = envCredentials["BLUEBIRDPS_API_SECRET"],
                    AccessToken = envCredentials["BLUEBIRDPS_ACCESS_TOKEN"],
                    AccessTokenSecret = envCredentials["BLUEBIRDPS_ACCESS_TOKEN_SECRET"]
                };

                if (envCredentials["BLUEBIRDPS_BEARER_TOKEN"] != null)
                {
                    oAuthCredentals.BearerToken = envCredentials["BLUEBIRDPS_BEARER_TOKEN"];
                }
            }
            else
            {
                try
                {
                    string credentialsFile = Credentials.ReadCredentialsFile();

                    WriteVerbose("Importing credentials from credentials file.");
                    oAuthCredentals = Credentials.ConvertCredentialsFromFile(credentialsFile);

                    WriteObject(oAuthCredentals);
                }
                catch
                {
                    WriteWarning("Unable to import Twitter authentication data from credentials file.");
                    WriteWarning("Please use the Set-TwitterAuthentication command to update the required API keys and secrets.");
                }
            }
        }
    }
}
