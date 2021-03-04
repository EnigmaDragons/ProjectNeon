
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
    
    public void LevelUp(PartyAdventureState p, Hero h, StatType s) 
        => SpendOnePoint(p, h, new StatAddends().With(s, Increases[s]));
    
    private void SpendOnePoint(PartyAdventureState p, Hero h, StatAddends stats)
    {
        if (h.Levels.LevelUpPoints < 1)
            return;
        
        p.ApplyLevelUpPoint(h, stats);
        Log.Info($"Leveled Up {h.Character.Name} - Remaining Points {h.Levels.LevelUpPoints}");
    }
}
