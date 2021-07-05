using BluebirdPS.Core;
using BluebirdPS.Models;
using System.IO;
using System.Management.Automation;

namespace BluebirdPS.Cmdlets.API.Authentication
{
    [Cmdlet(VerbsData.Export, "TwitterAuthentication")]
    public class ExportTwitterAuthenticationCommand : PSCmdlet
    {
        OAuth oAuthCredentals = Credentials.GetOrCreateInstance();

        protected override void ProcessRecord()
        {
            string action;
            if (!File.Exists(Config.credentialsPath))
            {
                action = "new";
                _ = File.Create(Config.credentialsPath);
            }
            else
            {
                action = "existing";
            }



            base.ProcessRecord();
        }

    }
}
