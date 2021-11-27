
public class AugmentLevelUpOption : LevelUpOption
{
    private readonly Equipment _e;

    public AugmentLevelUpOption(Equipment e) => _e = e;
    public string IconName => "Augment";
    public string Description => _e.Name;
    
    public void SelectAsLevelUp(Hero h)
    {
        h.Equipment.EquipPermanent(_e);
        h.RecordLevelUpPointSpent(-2);
        Message.Publish(new AutoSaveRequested());
    }

    public void ShowDetail()
    {
        throw new System.NotImplementedException();
    }

    public bool HasDetail => false;
    public bool IsFunctional => _e != null;
}
