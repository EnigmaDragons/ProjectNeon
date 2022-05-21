
public class SetBattleUiElementVisibility
{
    public bool ShouldShow { get; }
    public string UiElementName { get; }
    public string CallerId { get; }
    
    public SetBattleUiElementVisibility(string uiElementName, bool shouldShow, string callerId)
    {
        UiElementName = uiElementName;
        ShouldShow = shouldShow;
        CallerId = callerId;
    }
}
