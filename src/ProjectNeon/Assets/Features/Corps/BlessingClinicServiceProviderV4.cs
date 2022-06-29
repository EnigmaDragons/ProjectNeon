using System.Linq;

public class BlessingClinicServiceProviderV4 : ClinicServiceProvider
{
    private readonly PartyAdventureState _party;
    private readonly BlessingData[] _blessings;
    private ClinicServiceButtonData[] _generatedOptions;
    private bool[] _available;
    
    public BlessingClinicServiceProviderV4(PartyAdventureState party, BlessingData[] blessings)
    {
        _party = party;
        _blessings = blessings;
    }
    
    public string GetTitle() => "Blessings";

    public ClinicServiceButtonData[] GetOptions()
    {
        if (_generatedOptions == null)
        {
            _generatedOptions = new ClinicServiceButtonData[3];
            _available = new[] {true, true, true};
            var blessingChoices = _blessings
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
                .Where(x => x.blessing.Targets.Length > 0 && (x.blessingData.IsSingleTarget 
                    || (x.blessing.Targets.Count(target => target.Stats[x.blessingData.StatRequirement] >= x.blessingData.RequirementThreshold) > 1)))
                .ToArray()
                .Shuffled()
                .Take(3)
                .ToArray();
            for (int i = 0; i < blessingChoices.Length; i++)
            {
                var index = i;
                var d = blessingChoices[i].blessingData;
                _generatedOptions[i] = new ClinicServiceButtonData(
                    d.Name,
                    d.IsSingleTarget
                        ? string.Format(d.Description,
                            blessingChoices[i].blessing.Targets[0].DisplayName())
                        : d.Description,
                    1,
                    () =>
                    {
                        _party.AddBlessing(blessingChoices[index].blessing);
                        _available[index] = false;
                    }, d.Effect.AsArray(), 
                    "Tritoonico");
            }
        }
        for (var i = 0; i < _generatedOptions.Length; i++)
            _generatedOptions[i].Enabled = _available[i];
        return _generatedOptions;
    }
    
    public bool RequiresSelection() => false;
}