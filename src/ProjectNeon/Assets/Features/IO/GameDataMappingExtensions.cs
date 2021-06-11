using System.Linq;

public static class GameDataMappingExtensions
{
    public static GamePartyData GetData(this PartyAdventureState s)
        => new GamePartyData
        {
            Heroes = s.Heroes.Select(h => new GameHeroData
            {
                BaseHeroId = h.Character.Id,
            }).ToArray() 
        };
}
