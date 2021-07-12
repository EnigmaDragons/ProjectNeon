
public class OnHoverShowCorpLightDetails : CorpUiBase
{
    private Corp _corp;

    public void Show() => Message.Publish(new ShowCorpLightDetail(_corp));
    public void Hide() => Message.Publish(new HideCorpDetails());

    public override void Init(Corp c) => _corp = c;
}