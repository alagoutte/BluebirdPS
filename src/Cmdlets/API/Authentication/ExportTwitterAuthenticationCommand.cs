using BluebirdPS.Cmdlets.Base;
using BluebirdPS.Core;
using System.IO;
using System.Management.Automation;

namespace BluebirdPS.Cmdlets.API.Authentication
{
    [Cmdlet(VerbsData.Export, "TwitterAuthentication")]
    public class ExportTwitterAuthenticationCommand : BluebirdPSAuthCmdlet
    {
        protected override void ProcessRecord()
        {
            if (!File.Exists(configuration.CredentialsPath))
            {
                WriteVerbose($"Creating new credentials file: {configuration.CredentialsPath}");
                _ = File.Create(configuration.CredentialsPath);
            }
            else
            {
                WriteVerbose($"Saving to existing credentials file: {configuration.CredentialsPath}");
            }

            configuration.AuthLastExportDate = File.GetLastWriteTime(configuration.CredentialsPath);

            Credentials.SaveCredentialsToFile(oauth);
            WriteVerbose($"Credentials saved.");

        }
    }
}
