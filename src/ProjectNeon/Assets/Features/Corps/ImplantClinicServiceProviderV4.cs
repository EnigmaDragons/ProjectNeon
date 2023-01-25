using System;
using System.Collections.Generic;
using System.Linq;

public class ImplantClinicServiceProviderV4 : ClinicServiceProvider, ILocalizeTerms
{
    private static readonly Dictionary<StatType, int> StatAmounts = new Dictionary<StatType, int>
    {
        { StatType.MaxHP, 16 },
        { StatType.StartingShield, 8 },
        { StatType.Attack, 1 },
        { StatType.Magic, 1 },
        { StatType.Leadership, 1 },
        { StatType.Armor, 2 },
        { StatType.Resistance, 2 },
    };
    private static readonly StatType[] PowerStats = 
    {
        StatType.Attack,
        StatType.Magic,
        StatType.Leadership
    };
    private static readonly StatType[] TutorialStatTypes =
    {
        StatType.MaxHP,
        StatType.Attack,
        StatType.Armor,
        StatType.Resistance
    };
    
    private readonly PartyAdventureState _party;
    private readonly int _numOfImplants;
    private readonly DeterministicRng _rng;
    private readonly bool _isTutorial;
    private readonly Rarity[] _rarityChances;
    
    private ClinicServiceButtonData[] _generatedOptions;
    private bool[] _available;

    public ImplantClinicServiceProviderV4(PartyAdventureState party, int numOfImplants, DeterministicRng rng, bool isTutorial, Rarity[] rarityChances)
    {
        _party = party;
        _numOfImplants = numOfImplants;
        _rng = rng;
        _isTutorial = isTutorial;
        _rarityChances = rarityChances;
    }
    
    public string GetTitleTerm() => "Clinics/ImplantTitle";

    public ClinicServiceButtonData[] GetOptions()
    {
        if (_generatedOptions == null)
        {
            var newGeneratedOptions = new List<ClinicServiceButtonData>();
            for (int i = 0; i < _party.Heroes.Length; i++)
            {
                for (var ii = 0; ii < _numOfImplants / _party.Heroes.Length; ii++)
                {
                    var multiplier = (_numOfImplants / _party.Heroes.Length);
                    var option = GetOption(_party.Heroes[i], i * multiplier + ii);
                    while (newGeneratedOptions.Any(x => x.Description == option.Description))
                        option = GetOption(_party.Heroes[i], i * multiplier + ii);
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
        var loss = StatAmounts.Where(x => hero.PermanentStats[x.Key] >= x.Value && (!PowerStats.Contains(x.Key) || x.Key == hero.PrimaryStat) && (!_isTutorial || TutorialStatTypes.Contains(x.Key))).Random(_rng);
        var lossStat = loss.Key;
        var lossAmount = loss.Value;
        var gain = StatAmounts.Where(x => x.Key != loss.Key && (!PowerStats.Contains(x.Key) || x.Key == hero.PrimaryStat) && (!_isTutorial || TutorialStatTypes.Contains(x.Key))).Random(_rng);
        var gainStat = gain.Key;
        var gainAmount = gain.Value;
        var rarity = _rarityChances.Random(_rng);
        if (rarity == Rarity.Uncommon || rarity == Rarity.Epic)
            gainAmount = gainAmount * 2;
        if (rarity == Rarity.Rare || rarity == Rarity.Epic)
            lossAmount = 0;
        var cost = 0;
        if (rarity == Rarity.Common)
            cost = 1;
        else if (rarity == Rarity.Uncommon)
            cost = 2;
        else if (rarity == Rarity.Rare)
            cost = 3;
        else if (rarity == Rarity.Epic)
            cost = 4;
        var nameTerm = $"Clinics/Implant{lossStat.ToString()}{gainStat.ToString()}";
        return new ClinicServiceButtonData(
            nameTerm,
            rarity == Rarity.Rare || rarity == Rarity.Epic
                ? string.Format("Clinics/ImplantLossless".ToLocalized(), $"<b>{gainAmount} {gainStat.ToString().WithSpaceBetweenWords()}</b>", $"<b>{hero.NameTerm.ToLocalized()}</b>")
                : string.Format("Clinics/ImplantTradeOff".ToLocalized(), $"<b>{lossAmount} {lossStat.ToString().WithSpaceBetweenWords()}</b>", $"<b>{gainAmount} {gainStat.ToString().WithSpaceBetweenWords()}</b>", $"<b>{hero.NameTerm.ToLocalized()}</b>"),
        cost,
            () =>
            {
                if (_available.Length > index)
                {
                    _available[index] = false;
                    AdjustHero(hero, lossStat, lossAmount, gainStat, gainAmount);
                }
            }, 
            Array.Empty<EffectData>(),
            "Medigeneix",
            rarity,
            $"Medigeneix-{hero.NameTerm.ToEnglish()}-{nameTerm.ToEnglish()}");
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

    public string[] GetLocalizeTerms()
        => StatAmounts.SelectMany(loss => StatAmounts.Select(gain => $"Clinics/Implant{loss.Key.ToString()}{gain.Key.ToString()}"))
            .Concat(new[] { "Clinics/ImplantLossless", "Clinics/ImplantTradeOff", GetTitleTerm() })
            .ToArray();
}