using BluebirdPS.Models.Exceptions;
using System.Management.Automation;
using Tweetinvi.Exceptions;

namespace BluebirdPS.Core.Processors
{
    internal class GetTwitterErrorRecord
    {
        internal static ErrorRecord ProcessV2Error(TwitterException twitterException)
        {
            return new ErrorRecord(new BluebirdPSUnspecifiedException(twitterException.Content), twitterException.Source, ErrorCategory.NotSpecified, "Request Body");
        }
    }
}
