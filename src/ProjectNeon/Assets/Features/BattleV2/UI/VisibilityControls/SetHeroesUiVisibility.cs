public class SetHeroesUiVisibility
{
    public bool ShouldShow { get; }
    public string Component { get; }

    public SetHeroesUiVisibility(bool shouldShow, string component)
    {
        ShouldShow = shouldShow;
        Component = component;
    }    
}