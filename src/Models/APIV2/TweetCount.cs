using System;
using System.Globalization;

namespace BluebirdPS.Models.APIV2
{
    public class TweetCount : TwitterObject
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public long Count { get; set; }

        public TweetCount(dynamic input)
        {
            OriginalObject = input;

            Start = input.start.ToLocalTime();
            End = input.end.ToLocalTime();
            Count = input.tweet_count;
        }

        public override string ToString()
        {
            return $"Start: {Start.ToString("G",DateTimeFormatInfo.InvariantInfo)}, End: {End.ToString("G", DateTimeFormatInfo.InvariantInfo)}, Count: {Count}";
        }
    }

    public class TweetCountSummary
    {
        public string SearchString { get; set; }
        public string Granularity { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }        
        public long TotalCount { get; set; }

        public TweetCountSummary() { }
        public TweetCountSummary(string search, string granularity, DateTime starttime, DateTime endtime, long totalCount)
        {
            SearchString = search;
            Granularity = granularity;
            StartTime = starttime.ToLocalTime();
            EndTime = endtime.ToLocalTime();            
            TotalCount = totalCount;
        }

        public override string ToString()
        {
            return $"SearchString: {SearchString}, Granularity: {Granularity}, TotalCount: {TotalCount}, StartTime: {StartTime.ToString("G", DateTimeFormatInfo.InvariantInfo)}, EndTime: {EndTime.ToString("G", DateTimeFormatInfo.InvariantInfo)}";
        }
    }

}
