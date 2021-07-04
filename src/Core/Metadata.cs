using BluebirdPS.Models;
using System.Collections.Generic;
using System.Management.Automation;
using Tweetinvi.Core.Exceptions;
using Tweetinvi.Events;

namespace BluebirdPS.Core
{
    internal class Metadata
    {
        internal static Configuration Configuration { get; set; } = new Configuration();
        internal static List<ResponseData> History { get; set; } = new List<ResponseData>();
        internal static InvocationInfo InvocationInfo { get; set; }

        internal static List<BeforeExecutingRequestEventArgs> BeforeWaitingForRequestRateLimits { get; set; } = new List<BeforeExecutingRequestEventArgs>();
        internal static List<WaitingForRateLimitEventArgs> WaitingForRateLimit { get; set; } = new List<WaitingForRateLimitEventArgs>();
        internal static List<BeforeExecutingRequestEventArgs> BeforeExecutingRequest { get; set; } = new List<BeforeExecutingRequestEventArgs>();
        internal static List<AfterExecutingQueryEventArgs> AfterExecutingRequest { get; set; } = new List<AfterExecutingQueryEventArgs>();
        internal static List<ITwitterException> OnTwitterException { get; set; } = new List<ITwitterException>();
    }
}
