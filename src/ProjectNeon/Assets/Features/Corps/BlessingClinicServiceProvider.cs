using System.Linq;

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
    
    public string GetTitle() => "Blessings";

    public ClinicServiceButtonData[] GetOptions()
    {
        if (_generatedOptions == null)
            _generatedOptions = _blessings
                .Select(blessingData => new { blessingData, blessing = new Blessing
                {
                    Name = blessingData.Name,
                    Effect = blessingData.Effect, 
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
                .Where(x => x.blessing.Targets.Length > 0)
                .ToArray()
                .Shuffled()
                .Take(3)
                .Select(x => new ClinicServiceButtonData(
                    x.blessingData.Name, 
                    x.blessingData.IsSingleTarget ? string.Format(x.blessingData.Description, x.blessing.Targets[0].Name) : x.blessingData.Description, 
                    0,
                    () =>
                    {
                        _party.AddBlessing(x.blessing);
                        _hasProvidedService = true;
                    }))
                .ToArray();
        _generatedOptions.ForEach(x => x.Enabled = !_hasProvidedService);
        return _generatedOptions;
    }
}

