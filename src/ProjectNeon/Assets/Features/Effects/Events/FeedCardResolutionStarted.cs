public sealed class FeedCardResolutionStarted
{
    public string CardFeedType { get; }

    public FeedCardResolutionStarted(string feedType) => CardFeedType = feedType;
}
