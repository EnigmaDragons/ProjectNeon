using UnityEngine;

public interface LevelUpOption
{
    string IconName { get; }
    string Description { get; }
    public void SelectAsLevelUp(Hero h);
    public bool IsFunctional { get; }
    
    public bool HasDetail { get; }
    public void ShowDetail();
    
    public bool UseCustomOptionPresenter { get; }
    public GameObject CreatePresenter(LevelUpCustomPresenterContext ctx);
}
