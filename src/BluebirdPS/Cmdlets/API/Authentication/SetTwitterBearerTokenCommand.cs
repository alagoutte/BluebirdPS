using BluebirdPS.Cmdlets.Base;
using BluebirdPS.Core;
using System.Management.Automation;
using System.Security;

namespace BluebirdPS.Cmdlets.API.Authentication
{
    [Cmdlet(VerbsCommon.Set, "TwitterBearerToken")]
    public class SetTwitterBearerTokenCommand : AuthCmdlet
    {
        [Parameter()]
        public SecureString BearerToken { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                if (BearerToken == null)
                {
                    BearerToken = PSCommands.ReadHost("API Key", true);
                }

                oauth.BearerToken = PSCommands.ConvertFromSecureString(BearerToken, true);

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
