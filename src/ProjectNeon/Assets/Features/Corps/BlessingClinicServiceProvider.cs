﻿using System.Linq;
using System.Xml.Schema;

public class BlessingClinicServiceProvider : ClinicServiceProvider
{
    private readonly PartyAdventureState _party;
    private readonly BlessingData[] _blessings;
    private bool _hasProvidedService = false;
    private ClinicServiceButtonData[] _generatedOptions;

    public BlessingClinicServiceProvider(PartyAdventureState party, BlessingData[] blessings)
    {
        _party = party;
        _blessings = blessings;
    }
    
    public string GetTitleTerm() => "Blessings";

    public ClinicServiceButtonData[] GetOptions()
    {
        if (_generatedOptions == null)
            _generatedOptions = _blessings
                .Select(blessingData => new { blessingData, blessing = new Blessing
                {
                    Name = blessingData.Name,
                    Effect = blessingData.Effect, 
                    Duration = 3,
                    Targets = blessingData.IsSingleTarget 
                        ? _party.Heroes
                            .Where(x => x.Stats[blessingData.StatRequirement] >= blessingData.RequirementThreshold)
                            .Select(h => h.Character)
                            .ToArray()
                            .Shuffled()
                            .Take(1)
                            .ToArray()
                        : _party.Heroes.Select(h => h.Character).ToArray()
                }})
                .Where(x => x.blessing.Targets.Length > 0 && (x.blessingData.IsSingleTarget 
                    || (x.blessing.Targets.Count(target => target.Stats[x.blessingData.StatRequirement] >= x.blessingData.RequirementThreshold) > 1)))
                .ToArray()
                .Shuffled()
                .Take(3)
                .Select(x => new ClinicServiceButtonData(
                    x.blessingData.Name, 
                    x.blessingData.IsSingleTarget ? x.blessingData.Description.SafeFormat(x.blessing.Targets[0].NameTerm().ToLocalized()) : x.blessingData.Description, 
                    0,
                    () =>
                    {
                        _party.AddBlessing(x.blessing);
                        _hasProvidedService = true;
                    }, x.blessingData.Effect.AsArray(), "Tritoonico", Rarity.Starter, $"Tritoonico-{x.blessingData.Name}"))
                .ToArray();
        _generatedOptions.ForEach(x => x.Enabled = !_hasProvidedService);
        return _generatedOptions;
    }
    
    public bool RequiresSelection() => !_hasProvidedService;
}

