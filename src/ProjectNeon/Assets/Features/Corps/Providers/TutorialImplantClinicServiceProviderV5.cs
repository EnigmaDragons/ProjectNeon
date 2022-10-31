using System;
using System.Collections.Generic;
using System.Linq;

public class TutorialImplantClinicServiceProviderV5 : ClinicServiceProvider
{
    private static readonly Dictionary<StatType, int> _statAmounts = new Dictionary<StatType, int>
    {
        { StatType.MaxHP, 8 },
        { StatType.StartingShield, 4 },
        { StatType.Armor, 1 },
        { StatType.Resistance, 1 },
    };
    private static readonly Dictionary<StatType, string> _negativePrefix = new Dictionary<StatType, string>
    {
        { StatType.MaxHP, "Lifeblood" },
        { StatType.StartingShield, "Powered" },
        { StatType.Armor, "Fragilizing" },
        { StatType.Resistance, "Cursed" },
    };
    
    private static readonly Dictionary<StatType, string> _positiveSuffix = new Dictionary<StatType, string>
    {
        { StatType.MaxHP, "Vitality" },
        { StatType.StartingShield, "Barrier" },
        { StatType.Armor, "Protection" },
        { StatType.Resistance, "Ward" },
    };

    private readonly PartyAdventureState _party;
    private readonly int _numOfImplants;
    private ClinicServiceButtonData[] _generatedOptions;
    private bool[] _available;
    private DeterministicRng _rng;

    public TutorialImplantClinicServiceProviderV5(PartyAdventureState party, int numOfImplants, DeterministicRng rng)
    {
        _party = party;
        _numOfImplants = numOfImplants;
        _rng = rng;
    }
    
    public string GetTitleTerm() => "Available Implant Procedures";

    public ClinicServiceButtonData[] GetOptions()
    {
        if (_generatedOptions == null)
        {
            var newGeneratedOptions = new List<ClinicServiceButtonData>();
            for (int i = 0; i < _party.Heroes.Length; i++)
            {
                for (var ii = 0; ii < _numOfImplants / _party.Heroes.Length; ii++)
                {
                    var option = GetOption(_party.Heroes[i], i * 2 + ii);
                    while (newGeneratedOptions.Any(x => x.Description == option.Description))
                        option = GetOption(_party.Heroes[i], i * 2 + ii);
                    newGeneratedOptions.Add(option);   
                }
            }
            _generatedOptions = newGeneratedOptions.ToArray();
            _available = _generatedOptions.Select(x => true).ToArray();
        }
        for (var i = 0; i < _generatedOptions.Length; i++)
            _generatedOptions[i].Enabled = _available[i];
        return _generatedOptions;
    }

    private ClinicServiceButtonData GetOption(Hero hero, int index)
    {
        var loss = _statAmounts.Where(x => hero.PermanentStats[x.Key] >= x.Value).Random(_rng);
        var lossStat = loss.Key;
        var lossAmount = loss.Value;
        var gain = _statAmounts.Where(x => x.Key != loss.Key).Random(_rng);
        var gainStat = gain.Key;
        var gainAmount = gain.Value;
        return new ClinicServiceButtonData(
            $"{_negativePrefix[lossStat]} {_positiveSuffix[gainStat]}",
            $"Lose <b>{lossAmount} {lossStat.ToString().WithSpaceBetweenWords()}</b> to gain <b>{gainAmount} {gainStat.ToString().WithSpaceBetweenWords()}</b> on <b>{hero.NameTerm.ToEnglish()}</b>",
            1,
            () =>
            {
                AdjustHero(hero, lossStat, lossAmount, gainStat, gainAmount);
                _available[index] = false;
            }, Array.Empty<EffectData>(),
            "Medigeneix",
            Rarity.Common);
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
