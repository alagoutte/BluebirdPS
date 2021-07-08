using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Security;

namespace BluebirdPS.Core
{
    internal class PSCommands
    {
        internal static string GetContents(string path)
        {
            if (File.Exists(path))
            {
                using PowerShell pwsh = PowerShell.Create(RunspaceMode.CurrentRunspace);
                return pwsh.AddCommand("Get-Content")
                        .AddParameter("Path", path)
                        .Invoke<string>().ToList().First();
            }
            return null;
        }

        internal static SecureString ReadHost(string prompt, bool AsSecureString = false)
        {
            using PowerShell pwsh = PowerShell.Create(RunspaceMode.CurrentRunspace);
            return pwsh.AddCommand("Read-Host")
                    .AddParameter("Prompt", prompt)
                    .AddParameter("AsSecureString", AsSecureString)
                    .Invoke<SecureString>().ToList().First();
        }

        internal static string ConvertFromSecureString(SecureString input, bool asPlainText = false)
        {
            using PowerShell pwsh = PowerShell.Create(RunspaceMode.CurrentRunspace);
            return pwsh.AddCommand("ConvertFrom-SecureString")
                .AddParameter("SecureString", input)
                .AddParameter("AsPlainText", asPlainText)
                .Invoke<string>().ToList().First();
        }

        internal static SecureString ConvertToSecureString(string input, bool asPlainText = false)
        {
            using PowerShell pwsh = PowerShell.Create(RunspaceMode.CurrentRunspace);
            return pwsh.AddCommand("ConvertTo-SecureString")
                .AddParameter("String", input)
                .AddParameter("AsPlainText", asPlainText)
                .Invoke<SecureString>().ToList().First();
        }

        public static PSObject GetVariable(string variableName)
        {
            using (PowerShell pwsh = PowerShell.Create(RunspaceMode.CurrentRunspace))
            {
                var variableInfo = pwsh.AddCommand("Get-Variable").AddParameter("Name", variableName).AddParameter("ValueOnly", true).Invoke().ToList();
                foreach (PSObject variable in variableInfo)
                {
                    return variable;
                }
            }
            return null;
        }

        public static void SetVariable(string variableName, object value, string scope)
        {
            using PowerShell pwsh = PowerShell.Create(RunspaceMode.CurrentRunspace);
            pwsh.AddCommand("Set-Variable")
                .AddParameter("Name", variableName)
                .AddParameter("Value", value)
                .AddParameter("Scope", scope)
                .Invoke().ToList().First();
        }
    }
}
