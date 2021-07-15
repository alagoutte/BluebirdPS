using Newtonsoft.Json;

namespace BluebirdPS.Models
{
    //internal class OAuth
    //{
    //    [JsonProperty()]
    //    internal string ApiKey { get; set; }
    //    [JsonProperty()]
    //    internal string ApiSecret { get; set; }
    //    [JsonProperty()]
    //    internal string AccessToken { get; set; }
    //    [JsonProperty()]
    //    internal string AccessTokenSecret { get; set; }
    //    [JsonProperty()]
    //    internal string BearerToken { get; set; }
    //}

    public class OAuth
    {
        [JsonProperty()]
        public string ApiKey { get; set; }
        [JsonProperty()]
        public string ApiSecret { get; set; }
        [JsonProperty()]
        public string AccessToken { get; set; }
        [JsonProperty()]
        public string AccessTokenSecret { get; set; }
        [JsonProperty()]
        public string BearerToken { get; set; }

        internal bool IsNull(bool bearerToken = false)
        {
            if (string.IsNullOrEmpty(ApiKey) ||
                string.IsNullOrEmpty(ApiKey) ||
                string.IsNullOrEmpty(ApiKey) ||
                string.IsNullOrEmpty(ApiKey)
                )
            {
                return true;
            }

            if (bearerToken)
            {
                if (string.IsNullOrEmpty(BearerToken))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
