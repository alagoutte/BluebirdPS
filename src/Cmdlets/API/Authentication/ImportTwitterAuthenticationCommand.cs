using BluebirdPS.Cmdlets.Base;
using BluebirdPS.Core;
using System.Management.Automation;


namespace BluebirdPS.Cmdlets.Authentication
{
    [Cmdlet(VerbsData.Import, "TwitterAuthentication")]
    public class ImportTwitterAuthenticationCommand : BluebirdPSAuthCmdlet
    {
        protected override void ProcessRecord()
        {

            if (Credentials.HasCredentialsInEnvVars())
            {
                WriteVerbose("Importing credentials from environment variables.");
                oauth = Credentials.ReadCredentialsFromEnvVars();
            }
            else
            {
                WriteVerbose("Importing credentials from credentials file.");
                oauth = Credentials.ReadCredentialsFromFile();
            }

            if (oauth.IsNull())
            {
                WriteWarning($"Credentials were not found in environment variables (BLUEBIRDPS_*) or in { Config.credentialsPath}");
                WriteWarning("Please use the Set-TwitterAuthentication command to update the required API keys and secrets.");
                WriteWarning("For more information, see conceptual help topic: Get-Help about_BluebirdPS_Credentials");
            }
            else
            {
                if (PassThru)
                {
                    WriteObject(oauth);
                }
            }
        }
    }
}
