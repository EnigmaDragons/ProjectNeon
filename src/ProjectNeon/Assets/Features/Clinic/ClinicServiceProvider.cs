using System;

public interface ClinicServiceProvider
{
    string GetTitle();
    ClinicServiceButtonData[] GetOptions();
    bool RequiresSelection();
}

public class ClinicServiceButtonData
{
    public string Name { get; }
    public string Description { get; }
    public int Cost { get; }
    public Action Action { get; }
    public RulePanelContext RulesContext { get; }
    public string CorpName { get; }
    
    public bool Enabled { get; set; }

    public ClinicServiceButtonData(string name, string description, int cost, Action action, EffectData[] effects, string corpName)
    {
        Name = name;
        Description = description;
        Cost = cost;
        Action = action;
        RulesContext = new RulePanelContext(name, description, effects);
        CorpName = corpName;
    }
}