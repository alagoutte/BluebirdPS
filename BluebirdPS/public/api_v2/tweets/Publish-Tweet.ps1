function Publish-Tweet {
    [CmdletBinding(DefaultParameterSetName='Tweet')]
    param(
        [Parameter(Mandatory,ValueFromPipeline,Position='1')]
        [ValidateLength(1,10000)]
        [string]$TweetText,

        [Parameter()]
        [string]$ReplyToTweet,

        [Parameter(ParameterSetName='Tweet')]
        [string[]]$MediaId,

        [Parameter(Mandatory,ParameterSetName='TweetWithMedia')]
        [ValidateScript({Test-Path -Path $_})]
        [string]$Path,

        [Parameter(Mandatory,ParameterSetName='TweetWithMedia')]
        [ValidateSet('TweetImage','TweetVideo','TweetGif')]
        [string]$Category,

        [Parameter(ParameterSetName='TweetWithMedia')]
        [ValidateLength(1,1000)]
        [string]$AltImageText

    )

    # https://developer.twitter.com/en/docs/tweets/post-and-engage/api-reference/post-statuses-update
    # maximum of 4 pics, or 1 gif, or 1 video

    # count $TweetText characters
    # if the count is greater than allowed, suggest Send-TweetThread and fail

    if ($PSCmdlet.ParameterSetName -eq 'TweetWithMedia') {
        $SendMediaParams = @{
            Path = $Path
            Category = $Category
        }
        if ($PSBoundParameters.ContainsKey('AltImageText')) {
            $SendMediaParams.Add('AltImageText',$AltImageText)
        }
        $MediaId = Send-TwitterMedia @SendMediaParams | Select-Object -ExpandProperty media_id
    }

    $body = @{
        text = $TweetText
    }

    if ($PSBoundParameters.ContainsKey('ReplyToTweet')) {
        $reply = @{
            in_reply_to_tweet_id = $ReplyToTweet
        }
        $body.Add('reply', $reply)
    }

    if ($MediaId.Count -gt 0) {
        $media = @{
            media_ids = $MediaId
        }
        $body.Add('media', $media)
    }

    $Request = [TwitterRequest]@{
        HttpMethod = 'POST'
        Endpoint = 'https://api.twitter.com/2/tweets'
        ContentType = 'application/json'
        body = ($body | Convertto-json -Depth 10 -Compress)
    }

    try {
        $Tweet = Invoke-TwitterRequest -RequestParameters $Request
        Get-Tweet -Id $Tweet.id
    }
    catch {
        $PSCmdlet.ThrowTerminatingError($_)
    }
}
