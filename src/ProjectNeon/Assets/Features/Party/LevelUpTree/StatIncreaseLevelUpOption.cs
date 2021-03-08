using UnityEngine;

[CreateAssetMenu(menuName = "Hero/LevelUps/Stat")]
public class StatIncreaseLevelUpOption : HeroLevelUpOption
{
    [SerializeField] private StringVariable stat;
    [SerializeField] private int amount;

    protected override string Description => $"+{amount} {stat.Value}";
    
    protected override void Apply(Hero h) => h.ApplyLevelUpPoint(new StatAddends().WithRaw(stat.Value, amount));
}
