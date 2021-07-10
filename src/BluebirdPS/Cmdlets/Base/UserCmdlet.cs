using BluebirdPS.Models;
using System.Management.Automation;

namespace BluebirdPS.Cmdlets.Base
{
    public abstract class UserCmdlet : ClientCmdlet
    {
        [Parameter()]
        public SwitchParameter IncludeExpansions { get; set; }

        internal readonly ExpansionTypes ExpansionType = ExpansionTypes.User;
    }
}
