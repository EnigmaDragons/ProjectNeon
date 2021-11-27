using UnityEngine;

[CreateAssetMenu(menuName = "Hero/LevelUps/Stat")]
public class StatIncreaseLevelUpOption : StaticHeroLevelUpOption
{
    [SerializeField] private StringVariable stat;
    [SerializeField] private int amount;

    public override string IconName => stat.Value;
    public override string Description => $"+{amount} {stat.Value}";
    
    public override void Apply(Hero h) => h.AddToStats(new StatAddends().WithRaw(stat.Value, amount));

    public override void ShowDetail() {}
    public override bool HasDetail => false;
    public override bool IsFunctional => stat != null && amount > 0;
}
