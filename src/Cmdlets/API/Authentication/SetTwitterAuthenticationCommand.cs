using BluebirdPS.Cmdlets.Base;
using BluebirdPS.Core;
using BluebirdPS.Models;
using System.Management.Automation;
using System.Security;

namespace BluebirdPS.Cmdlets.API.Authentication
{
    [Cmdlet(VerbsCommon.Set, "TwitterAuthentication")]
    public class SetTwitterAuthenticationCommand : BluebirdPSAuthCmdlet
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
