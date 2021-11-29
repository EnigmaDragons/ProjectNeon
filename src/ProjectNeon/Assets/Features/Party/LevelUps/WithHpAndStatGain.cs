using System;
using UnityEngine;

public class LevelUpOptionWithHpAndStatGain : LevelUpOption
{
    private readonly PartyAdventureState _party;
    private readonly LevelUpOption _baseOption;
    private readonly int _hpGain;
    private readonly StatType _buffStat;
    private readonly int _buffStatAmount;

    public LevelUpOptionWithHpAndStatGain(PartyAdventureState party, LevelUpOption baseOption, int hpGain, StatType buffStat, int buffStatAmount)
    {
        _party = party;
        _baseOption = baseOption;
        _hpGain = hpGain;
        _buffStat = buffStat;
        _buffStatAmount = buffStatAmount;
    }
    
    public void SelectAsLevelUp(Hero h)
    {
        var perm = new InMemoryEquipment
        {
            Id = Rng.Int(Int32.MinValue, -1),
            Slot = EquipmentSlot.Permanent,
            Modifiers = new[]
            {
                new EquipmentStatModifier {ModifierType = StatMathOperator.Additive, Amount = _hpGain, StatType = StatType.MaxHP.ToString()},
                new EquipmentStatModifier {ModifierType = StatMathOperator.Additive, Amount = _buffStatAmount, StatType = _buffStat.ToString()}
            }
        };
        _party.Add(perm);
        h.ApplyPermanent(perm);
        _baseOption.SelectAsLevelUp(h);
    }
    
    public string IconName => _baseOption.IconName;
    public string Description => _baseOption.Description;
    public void ShowDetail() => _baseOption.ShowDetail();
    public bool HasDetail => _baseOption.HasDetail;
    public bool IsFunctional => _baseOption.IsFunctional;
    public bool UseCustomOptionPresenter => _baseOption.UseCustomOptionPresenter;
    public GameObject CreatePresenter(LevelUpCustomPresenterContext ctx) => _baseOption.CreatePresenter(ctx);
}
