
public class BattleSummaryReport
{
    public string[] Enemies { get; set; }
    public int TotalEnemyPowerLevel { get; set; }
    public EnemyTier FightTier { get; set; }
    
    public int AttritionHpChange { get; set; }
    public int AttritionInjuriesChange { get; set; }
    public int AttritionCreditsChange { get; set; }
    
    public string[] RewardCards { get; set; }
    public string[] RewardGear { get; set; }
    public int RewardXp { get; set; }
    public int RewardCredits { get; set; }
}