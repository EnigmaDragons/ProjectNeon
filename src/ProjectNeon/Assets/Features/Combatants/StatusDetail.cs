
public class StatusDetail
{
    public StatusTag Tag { get; }
    public Maybe<string> CustomText { get; }

    public StatusDetail(StatusTag tag)
        : this(tag, Maybe<string>.Missing()) {}
    public StatusDetail(StatusTag tag, Maybe<string> customText)
    {
        Tag = tag;
        CustomText = customText;
    }
}
