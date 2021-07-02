namespace BluebirdPS.Models.APIV2.Metrics.Tweet
{
    public class Public : BaseMetrics
    {
        public long RetweetCount { get; set; }
        public long ReplyCount { get; set; }
        public long LikeCount { get; set; }
        public long QuoteCount { get; set; }

        public Public() { }
        public Public(dynamic input)
        {
            RetweetCount = input.retweet_count;
            ReplyCount = input.reply_count;
            LikeCount = input.like_count;
            QuoteCount = input.quote_count;
            OriginalObject = input;
        }


    }

    public class NonPublic : BaseMetrics
    {
        public long ImpressionCount { get; set; }
        public long UrlLinkClicks { get; set; }
        public long UserProfileClicks { get; set; }

        public NonPublic() { }
        public NonPublic(dynamic input)
        {
            ImpressionCount = input.impression_count;
            UrlLinkClicks = input.url_link_clicks;
            UserProfileClicks = input.user_profile_clicks;
            OriginalObject = input;
        }
    }

    public class Organic : BaseMetrics
    {
        public long ImpressionCount { get; set; }
        public long LikeCount { get; set; }
        public long ReplyCount { get; set; }
        public long RetweetCount { get; set; }
        public long UrlLinkClicks { get; set; }
        public long UserProfileClicks { get; set; }

        public Organic() { }
        public Organic(dynamic input)
        {
            ImpressionCount = input.impression_count;
            LikeCount = input.like_count;
            ReplyCount = input.reply_count;
            RetweetCount = input.retweet_count;
            UrlLinkClicks = input.url_link_clicks;
            UserProfileClicks = input.user_profile_clicks;
            OriginalObject = input;
        }
    }

    public class Promoted : BaseMetrics
    {
        public long ImpressionCount { get; set; }
        public long LikeCount { get; set; }
        public long ReplyCount { get; set; }
        public long RetweetCount { get; set; }
        public long UrlLinkClicks { get; set; }
        public long UserProfileClicks { get; set; }

        public Promoted() { }
        public Promoted(dynamic input)
        {
            ImpressionCount = input.impression_count;
            LikeCount = input.like_count;
            ReplyCount = input.reply_count;
            RetweetCount = input.retweet_count;
            UrlLinkClicks = input.url_link_clicks;
            UserProfileClicks = input.user_profile_clicks;
            OriginalObject = input;
        }
    }
}
