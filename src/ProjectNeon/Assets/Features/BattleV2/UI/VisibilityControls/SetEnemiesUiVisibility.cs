
public class SetEnemiesUiVisibility
{
    public bool ShouldShow { get; }
    public string Component { get; }

    public SetEnemiesUiVisibility(bool shouldShow, string component)
    {
        ShouldShow = shouldShow;
        Component = component;
    } 
}
