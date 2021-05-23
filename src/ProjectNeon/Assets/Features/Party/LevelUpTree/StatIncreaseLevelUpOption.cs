using UnityEngine;

[CreateAssetMenu(menuName = "Hero/LevelUps/Stat")]
public class StatIncreaseLevelUpOption : HeroLevelUpOption
{
    [SerializeField] private StringVariable stat;
    [SerializeField] private int amount;

    public override string IconName => stat.Value;
    public override string Description => $"+{amount} {stat.Value}";
    
    public override void Apply(Hero h) => h.ApplyLevelUpPoint(new StatAddends().WithRaw(stat.Value, amount));
    public override void ShowDetail() {}
}
