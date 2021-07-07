using BluebirdPS.Cmdlets.Base;
using BluebirdPS.Core;
using BluebirdPS.Models;
using System.Management.Automation;


namespace BluebirdPS.Cmdlets.Authentication
{
    [Cmdlet(VerbsData.Import, "TwitterAuthentication")]
    public class ImportTwitterAuthenticationCommand : BluebirdPSCmdlet
    {
        protected override void ProcessRecord()
        {
            if (Credentials.EnvVarContainsCredentials())
            {
                WriteVerbose("Importing credentials from environment variables.");
                Credentials.GetCredentialsFromEnvVar();
            }
            else
            {
                try
                {
                    OAuth oAuthCredentials = Credentials.GetOrCreateInstance();

                    string credentialsFile = Credentials.ReadCredentialsFromFile();

                    WriteVerbose("Importing credentials from credentials file.");
                    oAuthCredentials = Credentials.GetCredentialsFromFile(credentialsFile);

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
