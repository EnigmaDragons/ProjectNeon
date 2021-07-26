using System;

[Serializable]
public class BattleSummaryReport
{
    public string[] enemies;
    public int totalEnemyPowerLevel;
    public string fightTier;
    
    public int attritionHpChange;
    public int attritionInjuriesChange;
    public int attritionCreditsChange;
    
    public string[] rewardCards;
    public string[] rewardGear;
    public int rewardXp;
    public int rewardCredits;
}