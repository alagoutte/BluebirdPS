using System;

namespace BluebirdPS.Models.APIV1
{
    public class RateLimitStatus
    {
        public string ResourceType { get; set; }
        public string Resource { get; set; }
        public string Limit { get; set; }
        public string Remaining { get; set; }
        public DateTime ResetTime { get; set; }

    }
}
