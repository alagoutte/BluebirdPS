using System.Management.Automation;
using System.Security;
using BluebirdPS.Core;
using BluebirdPS.Models;

namespace BluebirdPS.Cmdlets.AuthenticationCmdlets
{
    [Cmdlet(VerbsCommon.Set, "TwitterAuthentication")]
    public class SetTwitterAuthenticationCommand : AuthCmdlet
    {
        [Parameter()]
        public SecureString ApiKey { get; set; }
        [Parameter()]
        public SecureString ApiSecret { get; set; }
        [Parameter()]
        public SecureString AccessToken { get; set; }
        [Parameter()]
        public SecureString AccessTokenSecret { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                if (ApiKey == null)
                {
                    ApiKey = PSCommands.ReadHost("API Key", true);
                }
                if (ApiSecret == null)
                {
                    ApiSecret = PSCommands.ReadHost("API Secret", true);
                }
                if (AccessToken == null)
                {
                    AccessToken = PSCommands.ReadHost("Access Token", true);
                }
                if (AccessTokenSecret == null)
                {
                    AccessTokenSecret = PSCommands.ReadHost("Access Token Secret", true);
                }

                oauth = new OAuth()
                {
                    ApiKey = PSCommands.ConvertFromSecureString(ApiKey, true),
                    ApiSecret = PSCommands.ConvertFromSecureString(ApiSecret, true),
                    AccessToken = PSCommands.ConvertFromSecureString(AccessToken, true),
                    AccessTokenSecret = PSCommands.ConvertFromSecureString(AccessTokenSecret, true)
                };

                Credentials.SaveCredentialsToFile(oauth);
                WriteVerbose($"Credentials saved to {Config.credentialsPath}");

                if (PassThru)
                {
                    WriteObject(oauth);
                }
            }

            catch
            {
                WriteWarning("Failed to set credentials.");
            }
        }
    }
}
