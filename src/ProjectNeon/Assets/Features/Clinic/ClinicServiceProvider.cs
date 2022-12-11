using System;

public interface ClinicServiceProvider
{
    string GetTitleTerm();
    ClinicServiceButtonData[] GetOptions();
    bool RequiresSelection();
}

public class ClinicServiceButtonData
{
    public string NameTerm { get; }
    public string Description { get; }
    public int Cost { get; }
    public Action Action { get; }
    public RulePanelContext RulesContext { get; }
    public string CorpName { get; }
    public Rarity Rarity { get; }
    public string MetricDescription { get; }
    
    public bool Enabled { get; set; }

    public ClinicServiceButtonData(string nameTerm, string description, int cost, Action action, EffectData[] effects, string corpName, Rarity rarity, string metricDescription)
    {
        NameTerm = nameTerm;
        Description = description;
        Cost = cost;
        Action = action;
        RulesContext = new RulePanelContext(nameTerm, description, effects);
        CorpName = corpName;
        Rarity = rarity;
        MetricDescription = metricDescription;
    }

    public ClinicServiceButtonData WithAction(Action newAction)
    {
        return new ClinicServiceButtonData(NameTerm, Description, Cost, () =>
        {
            Action();
            newAction();
        }, RulesContext.Effects, CorpName, Rarity, MetricDescription) { Enabled = Enabled };
    }
}