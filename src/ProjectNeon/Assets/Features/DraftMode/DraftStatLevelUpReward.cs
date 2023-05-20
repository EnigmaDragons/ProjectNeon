using System;
using UnityEngine;

public class DraftStatLevelUpReward : LevelUpOption
{
    private readonly Action _action;
    
    public string IconName => "";
    public string Description { get; }
    public string EnglishDescription { get; }

    public DraftStatLevelUpReward(string description, string englishDescription, Action action)
    {
        _action = action;
        Description = description;
        EnglishDescription = englishDescription;
    }
    
    public void SelectAsLevelUp(Hero h)
    {
        _action();
        h.RecordLevelUpPointSpent(-3);
        Message.Publish(new AutoSaveRequested());
    }
    
    public bool IsFunctional => true;
    
    public bool HasDetail => false;
    public void ShowDetail() {}
    
    public bool UseCustomOptionPresenter => false;
    public GameObject CreatePresenter(LevelUpCustomPresenterContext ctx) => throw new System.NotImplementedException();
}
