using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ImplantClinicServiceProviderV4 : ClinicServiceProvider
{
    private static readonly Dictionary<StatType, int> _statAmounts = new Dictionary<StatType, int>
    {
        { StatType.MaxHP, 9 },
        { StatType.StartingShield, 6 },
        { StatType.Attack, 1 },
        { StatType.Magic, 1 },
        { StatType.Leadership, 1 },
        { StatType.Economy, 1 },
        { StatType.Armor, 2 },
        { StatType.Resistance, 2 },
    };
    private static readonly Dictionary<StatType, string> _negativePrefix = new Dictionary<StatType, string>
    {
        { StatType.MaxHP, "Unhealthy" },
        { StatType.StartingShield, "Powered" },
        { StatType.Attack, "Dulled" },
        { StatType.Magic, "Inhibiting" },
        { StatType.Leadership, "Clouded" },
        { StatType.Economy, "Costly" },
        { StatType.Armor, "Fragilizing" },
        { StatType.Resistance, "Cursed" },
    };
    private static readonly Dictionary<StatType, string> _positiveSuffix = new Dictionary<StatType, string>
    {
        { StatType.MaxHP, "Vitality" },
        { StatType.StartingShield, "Barrier" },
        { StatType.Attack, "Lethality" },
        { StatType.Magic, "Attunement" },
        { StatType.Leadership, "Focus" },
        { StatType.Economy, "Greed" },
        { StatType.Armor, "Protection" },
        { StatType.Resistance, "Ward" },
    };
    private static readonly StatType[] _powerStats = new[]
    {
        StatType.Attack,
        StatType.Magic,
        StatType.Leadership,
        StatType.Economy
    };
    private readonly PartyAdventureState _party;
    private ClinicServiceButtonData[] _generatedOptions;
    private bool[] _available;

    public ImplantClinicServiceProviderV4(PartyAdventureState party)
        => _party = party;
    
    public string GetTitle() => "Available Implant Procedures";

    public ClinicServiceButtonData[] GetOptions()
    {
        if (_generatedOptions == null)
        {
            _generatedOptions = new ClinicServiceButtonData[_party.Heroes.Length];
            for (int i = 0; i < _party.Heroes.Length; i++)
                _generatedOptions[i] = GetOption(_party.Heroes[i], i);
            _available = _generatedOptions.Select(x => true).ToArray();
        }
        for (var i = 0; i < _generatedOptions.Length; i++)
            _generatedOptions[i].Enabled = _available[i];
        return _generatedOptions;
    }

    private ClinicServiceButtonData GetOption(Hero hero, int index)
    {
        var loss = _statAmounts.Where(x => hero.PermanentStats[x.Key] >= x.Value && (!_powerStats.Contains(x.Key) || x.Key == hero.PrimaryStat)).ToArray().Shuffled().First();
        var lossStat = loss.Key;
        var lossAmount = loss.Value;
        var gain = _statAmounts.Where(x => x.Key != loss.Key && (!_powerStats.Contains(x.Key) || x.Key == hero.PrimaryStat)).ToArray().Shuffled().First();
        var gainStat = gain.Key;
        var gainAmount = gain.Value;
        return new ClinicServiceButtonData(
            $"{_negativePrefix[lossStat]} {_positiveSuffix[gainStat]}",
            $"Lose {lossAmount} {lossStat} to gain {gainAmount} {gainStat} on {hero.DisplayName}",
            1,
            () =>
            {
                AdjustHero(hero, lossStat, lossAmount, gainStat, gainAmount);
                _available[index] = false;
            });
    }

    private void AdjustHero(Hero hero, StatType lossStat, int lossAmount, StatType gainStat, int gainAmount)
    {
        hero.ApplyPermanent(new InMemoryEquipment
        {
            Name = "Implant",
            Slot = EquipmentSlot.Permanent,
            Modifiers = new[]
            {
                new EquipmentStatModifier { Amount = -lossAmount, StatType = lossStat.ToString(), ModifierType = StatMathOperator.Additive },
                new EquipmentStatModifier { Amount = gainAmount, StatType = gainStat.ToString(), ModifierType = StatMathOperator.Additive }
            }
        });
    }

    public bool RequiresSelection() => false;
}