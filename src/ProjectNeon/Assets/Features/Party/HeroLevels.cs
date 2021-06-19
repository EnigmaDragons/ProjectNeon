using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HeroLevels
{
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentXp = 0;
    [SerializeField] private int unspentLevelUpPoints = 0;
    [SerializeField] private int totalLevelUpPoints = 0;
    [SerializeField] private int nextXpThreshold = LevelThreshold(2);
    [SerializeField] private List<int> selectedLevelUpOptionsIds = new List<int>();

    private static int curveOffset = 50;
    private static int curveFactor = 50;
    
    public int CurrentLevel => currentLevel;
    public int Xp => currentXp;
    public int UnspentLevelUpPoints => unspentLevelUpPoints;
    public float NextLevelProgress => (float)XpTowardsNextLevelUp / XpRequiredForNextLevel;
    public int[] SelectedLevelUpOptionIds => selectedLevelUpOptionsIds.ToArray();

    public int XpRequiredForNextLevel => LevelThreshold(currentLevel + 1) - LevelThreshold(currentLevel);
    public int XpTowardsNextLevelUp => currentXp - LevelThreshold(currentLevel);

    public void AddXp(int xp)
    {
        currentXp += xp;
        while (currentXp >= nextXpThreshold)
        {
            currentLevel++;
            totalLevelUpPoints++;
            unspentLevelUpPoints++;
            nextXpThreshold = LevelThreshold(currentLevel + 1);
        }
    }

    public void LevelUp(int numLevels = 1)
    {
        for(var i = 0; i < numLevels; i++)
            AddXp(XpTowardsNextLevelUp);
    }

    public void RecordLevelUpCompleted(int levelUpOptionId)
    {
        if (unspentLevelUpPoints < 1)
        {
            Log.Error("Attempted to apply Level Up Perk with no Level Up Points");
            return;
        }

        unspentLevelUpPoints--;
        selectedLevelUpOptionsIds.Add(levelUpOptionId);
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
            unspentLevelUpPoints = unspentLevelUpPoints, 
            currentXp = currentXp, 
            nextXpThreshold = nextXpThreshold,
        };
        h.AddXp(xp);
        return h;
    }
}
