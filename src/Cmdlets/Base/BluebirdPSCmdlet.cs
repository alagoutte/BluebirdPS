using AutoMapper;
using BluebirdPS.Core;
using BluebirdPS.Models;
using BluebirdPS.Models.Exceptions;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using Tweetinvi;
using Tweetinvi.Models;
using Mapper = BluebirdPS.Core.Mapper;

namespace BluebirdPS.Cmdlets.Base
{
    public abstract class BluebirdPSCmdlet : PSCmdlet
    {
        internal static Configuration configuration = Config.GetOrCreateInstance();
        internal static List<ResponseData> history = History.GetOrCreateInstance();
    }

    public abstract class BluebirdPSAuthCmdlet : BluebirdPSCmdlet
    {
        internal static OAuth oauth = Credentials.GetOrCreateInstance();
    }

    public abstract class BluebirdPSClientCmdlet : BluebirdPSAuthCmdlet
    {
        [Parameter()]
        public SwitchParameter NoPagination { get; set; }

        internal static IMapper mapper = Mapper.GetOrCreateInstance();

        internal static TwitterClient client = Client.GetOrCreateInstance(GetTwitterCredentials(), configuration);

        private static TwitterCredentials GetTwitterCredentials()
        {
            if (oauth.IsNull())
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine($"Credentials were not found in environment variables(BLUEBIRDPS_ *) or in { Config.credentialsPath}");
                message.AppendLine("Please use the Set-TwitterAuthentication command to update the required API keys and secrets.");
                message.AppendLine("For more information, see conceptual help topic about_BluebirdPS_Credentials");

                throw new BluebirdPSNullCredentialsException(message.ToString());
            }

            return new TwitterCredentials(
                oauth.ApiKey,
                oauth.ApiSecret,
                oauth.AccessToken,
                oauth.AccessTokenSecret
                );

        }

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
