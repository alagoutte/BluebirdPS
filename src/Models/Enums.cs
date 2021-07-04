namespace BluebirdPS.Models
{
    public enum HttpMethod
    {
        GET, POST, DELETE, PATCH, PUT, HEAD, OPTIONS, TRACE
    }

    public enum ExpansionTypes
    {
        Tweet, User
    }

    public enum OAuthVersion
    {
        OAuth1a, OAuth2Bearer, Basic
    }

    public enum RateLimitAction
    {
        // type of output stream used to display a message
        // when the remaining calls are at the RateLimitThreshold or less

        Verbose,
        Warning,
        Error
    }

    public enum OutputType
    {
        BluebirdPS,
        JSON
    }
}
