﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ImplantClinicServiceProvider : ClinicServiceProvider
{
    private static readonly Dictionary<StatType, int> _statAmounts = new Dictionary<StatType, int>
    {
        { StatType.MaxHP, 6 },
        { StatType.StartingShield, 3 },
        { StatType.Attack, 1 },
        { StatType.Magic, 1 },
        { StatType.Leadership, 1 },
        { StatType.Economy, 1 },
        { StatType.Armor, 1 },
        { StatType.Resistance, 1 },
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
    private readonly PartyAdventureState _party;
    private bool _hasProvidedService = false;
    private ClinicServiceButtonData[] _generatedOptions;

    public ImplantClinicServiceProvider(PartyAdventureState party)
        => _party = party;
    
    public string GetTitleTerm() => "Available Implant Procedures";

    public ClinicServiceButtonData[] GetOptions()
    {
        if (_generatedOptions == null)
            _generatedOptions = _party.Heroes.Select(GetOption).ToArray();
        _generatedOptions.ForEach(x => x.Enabled = !_hasProvidedService);
        return _generatedOptions;
    }

    private ClinicServiceButtonData GetOption(Hero hero)
    {
        var statsToAdjust = _statAmounts
            .Where(x => hero.PermanentStats[x.Key] >= x.Value)
            .TakeRandom(2);
        var lossStat = statsToAdjust[0].Key;
        var lossAmount = statsToAdjust[0].Value;
        var gainStat = statsToAdjust[1].Key;
        var gainAmount = statsToAdjust[1].Value;
        var heroNameEnglish = hero.NameTerm.ToEnglish();
        var serviceTitle = $"{_negativePrefix[lossStat]} {_positiveSuffix[gainStat]}";
        return new ClinicServiceButtonData(
            serviceTitle,
            $"Lose {lossAmount} {lossStat.ToString().WithSpaceBetweenWords()} to gain {gainAmount} {gainStat.ToString().WithSpaceBetweenWords()} on {heroNameEnglish}",
            CalculateCost(hero, lossStat, lossAmount, gainStat, gainAmount),
            () => AdjustHero(hero, lossStat, lossAmount, gainStat, gainAmount),
            Array.Empty<EffectData>(),
            "Medigeneix",
            Rarity.Starter,
            $"Medigeneix-{heroNameEnglish}-{serviceTitle}");
    }

    public int CalculateCost(Hero hero, StatType lossStat, int lossAmount, StatType gainStat, int gainAmount) 
        => 30 + CalculateLoseCost(hero, lossStat, lossAmount) + CalculateGainCost(hero, gainStat, gainAmount);
    private int CalculateLoseCost(Hero hero, StatType stat, int amount) 
        => - (int)Mathf.Clamp((hero.LevelUpsAndImplants[stat] / amount) * 3f, -10f, 10f);
    private int CalculateGainCost(Hero hero, StatType stat, int amount) 
        => (int)Mathf.Clamp((hero.LevelUpsAndImplants[stat] / amount) * 3f, -10f, 10f);

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
        _hasProvidedService = true;
    }

    public bool RequiresSelection() => false;
}