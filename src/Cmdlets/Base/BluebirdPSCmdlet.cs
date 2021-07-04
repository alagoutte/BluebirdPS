using System.Management.Automation;
using AutoMapper;
using BluebirdPS.Core;
using BluebirdPS.Models;
using Tweetinvi;

namespace BluebirdPS.Cmdlets.Base
{
    public abstract class BluebirdPSCmdlet : PSCmdlet
    {
        [Parameter()]
        public SwitchParameter NoPagination { get; set; }

        internal static IMapper mapper = BluebirdPSMapper.GetOrCreateInstance();

        internal static TwitterClient client = BluebirdPSClient.GetOrCreateInstance();
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
