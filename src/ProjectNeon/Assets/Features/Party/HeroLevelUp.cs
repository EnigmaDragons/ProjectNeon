
using System.Collections.Generic;

public class HeroLevelUp
{
    private int maxHpPerPoint = 5;
    private int toughnessPerPoint = 3;
    private int attackPerPoint = 1;
    private int magicPerPoint = 1;
    private int armorPerPoint = 2;
    private int resistPerPoint = 2;

    public Dictionary<StatType, int> Increases => new Dictionary<StatType,int>
    {
        { StatType.MaxHP, maxHpPerPoint },
        { StatType.Toughness, toughnessPerPoint },
        { StatType.Attack, attackPerPoint },
        { StatType.Magic, magicPerPoint },
        { StatType.Armor, armorPerPoint },
        { StatType.Resistance, resistPerPoint },
    };
    
    public void LevelUp(Hero h, StatType s) => SpendOnePoint(h, new StatAddends().With(s, Increases[s]));
    
    private void SpendOnePoint(Hero h, StatAddends stats)
    {
        if (h.LevelUpPoints < 1)
            return;
        
        h.AdjustLevelUpPoints(-1);
        h.AddLevelUpStat(stats);
    }
}
