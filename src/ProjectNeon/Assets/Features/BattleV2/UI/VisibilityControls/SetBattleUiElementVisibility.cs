
public class SetBattleUiElementVisibility
{
    public bool ShouldShow { get; }
    public string UiElementName { get; }
    
    public SetBattleUiElementVisibility(string uiElementName, bool shouldShow)
    {
        UiElementName = uiElementName;
        ShouldShow = shouldShow;
    }
}
