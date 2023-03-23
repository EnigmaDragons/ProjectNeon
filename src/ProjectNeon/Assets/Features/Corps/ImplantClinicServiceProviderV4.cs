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
    private readonly DeterminedNodeInfo _nodeInfo;
    
    private ClinicServiceButtonData[] _generatedOptions;
    private bool[] _available;

    public ImplantClinicServiceProviderV4(PartyAdventureState party, int numOfImplants, DeterministicRng rng, bool isTutorial, Rarity[] rarityChances, DeterminedNodeInfo nodeInfo)
    {
        _party = party;
        _numOfImplants = numOfImplants;
        _rng = rng;
        _isTutorial = isTutorial;
        _rarityChances = rarityChances;
        _nodeInfo = nodeInfo;
    }
    
    public string GetTitleTerm() => "Clinics/ImplantTitle";

    public ClinicServiceButtonData[] GetOptions()
    {
        if (_generatedOptions == null)
        {
            if (_nodeInfo.Implants.IsMissing)
            {
                var newGeneratedOptionData = new List<ImplantData>();
                for (int i = 0; i < _party.Heroes.Length; i++)
                {
                    for (var ii = 0; ii < _numOfImplants / _party.Heroes.Length; ii++)
                    {
                        var option = GetOptionData(_party.Heroes[i]);
                        while (newGeneratedOptionData.Any(x => x.LossStat == option.LossStat && x.GainStat == option.GainStat))
                            option = GetOptionData(_party.Heroes[i]);
                        newGeneratedOptionData.Add(option);   
                    }
                }
                _nodeInfo.Implants = newGeneratedOptionData.ToArray();
                Message.Publish(new SaveDeterminationsRequested());
            }
            var newGeneratedOptions = new List<ClinicServiceButtonData>();
            for (var i = 0; i < _nodeInfo.Implants.Value.Length; i++)
                newGeneratedOptions.Add(GetOption(_nodeInfo.Implants.Value[i], _party.Heroes, i));
            _generatedOptions = newGeneratedOptions.ToArray();
            _available = _generatedOptions.Select(x => true).ToArray();
        }
        for (var i = 0; i < _generatedOptions.Length; i++)
            _generatedOptions[i].Enabled = _available[i];
        return _generatedOptions;
    }

    private ImplantData GetOptionData(Hero hero)
    {
        var lossStat = StatAmounts.Where(x => hero.PermanentStats[x.Key] >= x.Value && (!PowerStats.Contains(x.Key) || x.Key == hero.PrimaryStat) && (!_isTutorial || TutorialStatTypes.Contains(x.Key))).Random(_rng).Key;
        var gainStat = StatAmounts.Where(x => x.Key != lossStat && (!PowerStats.Contains(x.Key) || x.Key == hero.PrimaryStat) && (!_isTutorial || TutorialStatTypes.Contains(x.Key))).Random(_rng).Key;
        return new ImplantData()
        {
            HeroId = hero.Character.Id,
            LossStat = lossStat,
            GainStat = gainStat,
            Rarity = _rarityChances.Random(_rng)
        };
    }

    private ClinicServiceButtonData GetOption(ImplantData implant, Hero[] heroes, int index)
    {
        var hero = heroes.First(x => x.Character.Id == implant.HeroId);
        var loss = StatAmounts.First(x => x.Key == implant.LossStat);
        var lossStat = loss.Key;
        var lossAmount = loss.Value;
        var gain = StatAmounts.First(x => x.Key == implant.GainStat);
        var gainStat = gain.Key;
        var gainAmount = gain.Value;
        if (implant.Rarity == Rarity.Uncommon || implant.Rarity == Rarity.Epic)
            gainAmount = gainAmount * 2;
        if (implant.Rarity == Rarity.Rare || implant.Rarity == Rarity.Epic)
            lossAmount = 0;
        var cost = 0;
        if (implant.Rarity == Rarity.Common)
            cost = 1;
        else if (implant.Rarity == Rarity.Uncommon)
            cost = 2;
        else if (implant.Rarity == Rarity.Rare)
            cost = 3;
        else if (implant.Rarity == Rarity.Epic)
            cost = 4;
        var nameTerm = $"Clinics/Implant{lossStat.ToString()}{gainStat.ToString()}";
        return new ClinicServiceButtonData(
            nameTerm,
            implant.Rarity == Rarity.Rare || implant.Rarity == Rarity.Epic
                ? "Clinics/ImplantLossless".ToLocalized().SafeFormatWithDefault("Gain {0} {1} on {2}", $"<b>{gainAmount}</b>", $"<b>{gainStat.GetLocalizedString()}</b>", $"<b>{hero.NameTerm.ToLocalized()}</b>")
                : "Clinics/ImplantTradeOff".ToLocalized().SafeFormatWithDefault("Lose {0} {1} to gain {2} {3} on {4}", $"<b>{lossAmount}</b>", $"<b>{lossStat.GetLocalizedString()}</b>", $"<b>{gainAmount}</b>", $"<b>{gainStat.GetLocalizedString()}</b>", $"<b>{hero.NameTerm.ToLocalized()}</b>"),
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
            implant.Rarity,
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