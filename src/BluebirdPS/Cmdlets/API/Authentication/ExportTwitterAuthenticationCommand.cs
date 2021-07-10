using BluebirdPS.Cmdlets.Base;
using BluebirdPS.Core;
using System.IO;
using System.Management.Automation;

namespace BluebirdPS.Cmdlets.API.Authentication
{
    [Cmdlet(VerbsData.Export, "TwitterAuthentication")]
    public class ExportTwitterAuthenticationCommand : AuthCmdlet
    {
        protected override void ProcessRecord()
        {
            if (!File.Exists(Configuration.CredentialsPath))
            {
                WriteVerbose($"Creating new credentials file: {Configuration.CredentialsPath}");
                _ = File.Create(Configuration.CredentialsPath);
            }
            else
            {
                WriteVerbose($"Saving to existing credentials file: {Configuration.CredentialsPath}");
            }

            Configuration.AuthLastExportDate = File.GetLastWriteTime(Configuration.CredentialsPath);

            Credentials.SaveCredentialsToFile(oauth);
            WriteVerbose($"Credentials saved to {Config.credentialsPath}");

            if (PassThru)
            {
                WriteObject(oauth);
            }
        }
    }
}
