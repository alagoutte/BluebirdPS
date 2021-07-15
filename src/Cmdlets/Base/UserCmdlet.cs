using System.Management.Automation;
using BluebirdPS.Models;

namespace BluebirdPS.Cmdlets.Base
{
    public abstract class UserCmdlet : ClientCmdlet
    {
        [Parameter()]
        public SwitchParameter IncludeExpansions { get; set; }

        internal readonly ExpansionTypes ExpansionType = ExpansionTypes.User;
    }
}
