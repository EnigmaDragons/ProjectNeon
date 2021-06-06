using System;
using UnityEngine;

[Serializable]
public class HeroLevels
{
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentXp = 0;
    [SerializeField] private int levelUpPoints = 0;
    [SerializeField] private int nextXpThreshold = LevelThreshold(2);

    private static int curveOffset = 50;
    private static int curveFactor = 50;
    private IStats _levelUpStats = new StatAddends();
    
    public int CurrentLevel => currentLevel;
    public int Xp => currentXp;
    public int LevelUpPoints => levelUpPoints;
    public float NextLevelProgress => (float)XpTowardsNextLevelUp / XpRequiredForNextLevel; 

    public int XpRequiredForNextLevel => LevelThreshold(currentLevel + 1) - LevelThreshold(currentLevel);
    public int XpTowardsNextLevelUp => currentXp - LevelThreshold(currentLevel);
    public IStats LevelUpStats => _levelUpStats;

    public void AddXp(int xp)
    {
        currentXp += xp;
        while (currentXp >= nextXpThreshold)
        {
            currentLevel++;
            levelUpPoints++;
            nextXpThreshold = LevelThreshold(currentLevel + 1);
        }
    }

    public void LevelUp(int numLevels = 1)
    {
        for(var i = 0; i < numLevels; i++)
            AddXp(XpTowardsNextLevelUp);
    }

    public void RecordLevelUpCompleted()
    {
        if (levelUpPoints < 1)
        {
            Log.Error("Attempted to apply Level Up Perk with no Level Up Points");
            return;
        }

        levelUpPoints--;
    }

    public void ApplyLevelUpStats(StatAddends s)
    {
        if (levelUpPoints < 1)
            return;
        
        _levelUpStats = _levelUpStats.Plus(s);
        levelUpPoints--;
    }

    private static int LevelThreshold(int level) 
        => level == 1 
            ? 0 
            : NextLevelUpNumber(level - 1) * curveFactor + curveOffset;

    private static int NextLevelUpNumber(int value)
    {
        if (value == 0)
            return 0;
        
        var n = value;
        var res = 1;
        while (n != 1) {
            res = res + n;
            n = n - 1;
        }
        return res;
    }
    
    public HeroLevels PreviewChange(int xp)
    {
        var h = new HeroLevels
        {
            currentLevel = currentLevel, 
            levelUpPoints = levelUpPoints, 
            currentXp = currentXp, 
            nextXpThreshold = nextXpThreshold
        };
        h.AddXp(xp);
        return h;
    }
}
