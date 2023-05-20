using UnityEngine;

[CreateAssetMenu(menuName = "Hero/LevelUps/Stat")]
public class StatIncreaseLevelUpOption : StaticHeroLevelUpOption
{
    [SerializeField] private StringVariable stat;
    [SerializeField] private int amount;

    public override string IconName => stat.Value;
    public override string Description => $"+{amount} {$"Stats/Stat-{stat.Value}".ToLocalized()}";
    public override string EnglishDescription => $"+{amount} {$"Stats/Stat-{stat.Value}".ToEnglish()}";
    
    public override void Apply(Hero h) => h.AddToStats(new StatAddends().WithRaw(stat.Value, amount));

    public override void ShowDetail() {}
    public override bool HasDetail => false;
    public override bool IsFunctional => stat != null && amount > 0;

    public override bool UseCustomOptionPresenter => false;
    public override GameObject CreatePresenter(LevelUpCustomPresenterContext ctx) => throw new System.NotImplementedException();
}
