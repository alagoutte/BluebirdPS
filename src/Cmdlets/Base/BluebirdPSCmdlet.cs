using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections;
using System.Linq;
using BluebirdPS.Models;

namespace BluebirdPS.Cmdlets.Base
{
    public abstract class BluebirdPSCmdlet : PSCmdlet
    {
        [Parameter()]
        public SwitchParameter NoPagination { get; set; }

    }

    public abstract class BluebirdPSUserCmdlet : BluebirdPSCmdlet
    {
        [Parameter()]
        public SwitchParameter IncludeExpansions { get; set; }

        internal readonly ExpansionTypes ExpansionType = ExpansionTypes.User;
    }

    public abstract class BluebirdPSTweetCmdlet : BluebirdPSCmdlet
    {
        [Parameter()]
        public SwitchParameter NonPublicMetrics { get; set; }
        [Parameter()]
        public SwitchParameter PromotedMetrics { get; set; }
        [Parameter()]
        public SwitchParameter OrganicMetrics { get; set; }
        [Parameter()]
        public SwitchParameter IncludeExpansions { get; set; }

        internal readonly ExpansionTypes ExpansionType = ExpansionTypes.Tweet;
    }
}
