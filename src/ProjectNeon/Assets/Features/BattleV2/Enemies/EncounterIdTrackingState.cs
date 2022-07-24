using System.Linq;

public static class EncounterIdTrackingState
{
    private static Maybe<string> _encounterId = Maybe<string>.Missing();
    
    public static void StoreEncounterId(string id) => _encounterId = id;

    public static void MarkEncounterFinished()
    { 
        _encounterId.IfPresent(id => CurrentGameData.Mutate(x => x.Fights.Encounters = x.Fights.Encounters.Concat(id).ToArray()));
        _encounterId = Maybe<string>.Missing();
    }
}
