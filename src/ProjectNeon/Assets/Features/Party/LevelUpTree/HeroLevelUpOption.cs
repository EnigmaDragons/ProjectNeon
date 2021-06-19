using UnityEngine;

public abstract class HeroLevelUpOption : ScriptableObject
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
}
