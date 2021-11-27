using UnityEngine;

public abstract class StaticHeroLevelUpOption : ScriptableObject, LevelUpOption
{
    [SerializeField, UnityEngine.UI.Extensions.ReadOnly] public int id;
    
    public int Id => id;
    public abstract string IconName { get; }
    public abstract string Description { get; }

    public void SelectAsLevelUp(Hero h)
    {
        Apply(h);
        h.RecordLevelUpPointSpent(Id);
        Message.Publish(new AutoSaveRequested());
    }
    
    public abstract void Apply(Hero h);
    public abstract void ShowDetail();
    public abstract bool HasDetail { get; }
    public abstract bool IsFunctional { get; }
    public abstract bool UseCustomOptionPresenter { get; }
    public abstract GameObject CreatePresenter(LevelUpCustomPresenterContext ctx);
}
