using System;
using System.Collections.Generic;
using System.Linq;

public class ImplantClinicServiceProviderV4 : ClinicServiceProvider
{
    private static readonly Dictionary<StatType, int> StatAmounts = new Dictionary<StatType, int>
    {
        { StatType.MaxHP, 16 },
        { StatType.StartingShield, 8 },
        { StatType.Attack, 1 },
        { StatType.Magic, 1 },
        { StatType.Leadership, 1 },
        { StatType.Economy, 1 },
        { StatType.Armor, 2 },
        { StatType.Resistance, 2 },
    };
    private static readonly Dictionary<StatType, string> NegativePrefix = new Dictionary<StatType, string>
    {
        { StatType.MaxHP, "Lifeblood" },
        { StatType.StartingShield, "Powered" },
        { StatType.Attack, "Dulled" },
        { StatType.Magic, "Inhibiting" },
        { StatType.Leadership, "Clouded" },
        { StatType.Economy, "Costly" },
        { StatType.Armor, "Fragilizing" },
        { StatType.Resistance, "Cursed" },
    };
    private static readonly Dictionary<StatType, string> PositiveSuffix = new Dictionary<StatType, string>
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
    private static readonly StatType[] PowerStats = {
        StatType.Attack,
        StatType.Magic,
        StatType.Leadership,
        StatType.Economy
    };
    
    private readonly PartyAdventureState _party;
    private readonly int _numOfImplants;
    private readonly DeterministicRng _rng;
    
    private ClinicServiceButtonData[] _generatedOptions;
    private bool[] _available;

    public ImplantClinicServiceProviderV4(PartyAdventureState party, int numOfImplants, DeterministicRng rng)
    {
        _party = party;
        _numOfImplants = numOfImplants;
        _rng = rng;
    }
    
    public string GetTitle() => "Available Implant Procedures";

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
        var loss = StatAmounts.Where(x => hero.PermanentStats[x.Key] >= x.Value && (!PowerStats.Contains(x.Key) || x.Key == hero.PrimaryStat)).Random(_rng);
        var lossStat = loss.Key;
        var lossAmount = loss.Value;
        var gain = StatAmounts.Where(x => x.Key != loss.Key && (!PowerStats.Contains(x.Key) || x.Key == hero.PrimaryStat)).Random(_rng);
        var gainStat = gain.Key;
        var gainAmount = gain.Value;
        return new ClinicServiceButtonData(
            $"{NegativePrefix[lossStat]} {PositiveSuffix[gainStat]}",
            $"Lose <b>{lossAmount} {lossStat.ToString().WithSpaceBetweenWords()}</b> to gain <b>{gainAmount} {gainStat.ToString().WithSpaceBetweenWords()}</b> on <b>{hero.DisplayName}</b>",
            1,
            () =>
            {
                if (_available.Length > index)
                {
                    _available[index] = false;
                    AdjustHero(hero, lossStat, lossAmount, gainStat, gainAmount);
                }
            }, Array.Empty<EffectData>(),
            "Medigeneix");
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