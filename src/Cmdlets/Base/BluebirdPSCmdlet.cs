using AutoMapper;
using BluebirdPS.Core;
using BluebirdPS.Models;
using System.Collections.Generic;
using System.Management.Automation;
using Tweetinvi;
using Mapper = BluebirdPS.Core.Mapper;

namespace BluebirdPS.Cmdlets.Base
{
    public abstract class BluebirdPSCmdlet : PSCmdlet
    {
        internal static Configuration configuration = Config.GetOrCreateInstance();
        internal static List<ResponseData> history = History.GetOrCreateInstance();
    }

    public abstract class BluebirdPSClientCmdlet : BluebirdPSCmdlet
    {
        [Parameter()]
        public SwitchParameter NoPagination { get; set; }

        internal static IMapper mapper = Mapper.GetOrCreateInstance();
        internal static TwitterClient client = Client.GetOrCreateInstance();
    }

    public abstract class BluebirdPSUserCmdlet : BluebirdPSClientCmdlet
    {
        [Parameter()]
        public SwitchParameter IncludeExpansions { get; set; }

        internal readonly ExpansionTypes ExpansionType = ExpansionTypes.User;
    }

    public abstract class BluebirdPSTweetCmdlet : BluebirdPSClientCmdlet
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
