using System.Management.Automation;
using BluebirdPS.Core;
using BluebirdPS.Models;

namespace BluebirdPS.Cmdlets.Base
{
    public abstract class AuthCmdlet : BaseCmdlet
    {
        [Parameter()]
        public SwitchParameter PassThru { get; set; }

        internal static OAuth oauth = Credentials.GetOrCreateInstance();
    }
}
