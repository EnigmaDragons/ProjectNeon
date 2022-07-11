using System;
using UnityEngine;

public class DraftStatLevelUpReward : LevelUpOption
{
    private readonly Action _action;
    
    public string IconName { get; } = "";
    public string Description { get; }

    public DraftStatLevelUpReward(string description, Action action)
    {
        _action = action;
        Description = description;
    }
    
    public void SelectAsLevelUp(Hero h)
    {
        _action();
        h.RecordLevelUpPointSpent(-3);
        Message.Publish(new AutoSaveRequested());
    }
    
    public bool IsFunctional { get; } = true;
    
    public bool HasDetail { get; } = false;
    public void ShowDetail() => throw new System.NotImplementedException();
    
    public bool UseCustomOptionPresenter { get; } = false;
    public GameObject CreatePresenter(LevelUpCustomPresenterContext ctx) => throw new System.NotImplementedException();
}
