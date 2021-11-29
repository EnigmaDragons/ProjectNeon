using System;
using UnityEngine;

public class AugmentLevelUpOption : LevelUpOption
{
    private readonly PartyAdventureState _party;
    private readonly Equipment _e;

    public AugmentLevelUpOption(PartyAdventureState party, Equipment e)
    {
        _party = party;
        _e = e;
    }

    public string IconName => "Augment";
    public string Description => _e.Name;
    
    public void SelectAsLevelUp(Hero h)
    {
        _party.Add(_e);
        h.Equipment.EquipPermanent(_e);
        h.RecordLevelUpPointSpent(-2);
        Message.Publish(new AutoSaveRequested());
    }

    public void ShowDetail() => throw new NotImplementedException();
    public bool HasDetail => false;
    public bool IsFunctional => _e != null;
    public bool UseCustomOptionPresenter => false;
    public GameObject CreatePresenter(LevelUpCustomPresenterContext ctx) => throw new System.NotImplementedException();
}
