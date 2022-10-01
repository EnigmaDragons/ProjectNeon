
public class RulePanelContext
{
    public string SourceName { get; }
    public string Description { get; }
    public EffectData[] Effects { get; }

    public RulePanelContext(string sourceName, string description, EffectData[] effects)
    {
        SourceName = sourceName;
        Description = description;
        Effects = effects;
    }
}
