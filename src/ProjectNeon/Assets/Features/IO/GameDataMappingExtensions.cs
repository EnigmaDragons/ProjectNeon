using System.Linq;

public static class GameDataMappingExtensions
{
    public static GamePartyData GetData(this PartyAdventureState s)
        => new GamePartyData
        {
            Credits = s.Credits,
            Heroes = s.Heroes
                .Select(h => new GameHeroData
                {
                    BaseHeroId = h.Character.Id,
                    BasicCardId = h.BasicCard.Id,
                    Health = h.Health,
                    Levels = h.Levels,
                    Deck = new GameDeckData { CardIds = h.Deck.Cards.Select(c => c.Id).ToArray() },
                    EquipmentIdNames = h.Equipment.All.Select(x => new GameEquipmentIdName { Id = x.Id, Name = x.Name }).ToArray()
                }).ToArray(),
            CardIds = s.Cards.AllCards
                .SelectMany(cSlot => Enumerable.Range(0, cSlot.Value).Select(_ => cSlot.Key.Id))
                .ToArray(),
            Equipment = s.Equipment.All.Select(x => x.GetData()).ToArray()
        };

    public static GameAdventureProgressData GetData(this AdventureProgress2 p)
        => new GameAdventureProgressData
        {
            AdventureId = p.CurrentAdventureId,
            Type = GameAdventureProgressType.V2,
            CurrentChapterIndex = p.CurrentChapterIndex,
            CurrentStageSegmentIndex = p.CurrentStageSegmentIndex
        };

    public static GameMapData GetData(this CurrentGameMap3 map)
        => new GameMapData
        {
            GameMapId = map.CurrentMap.id,
            CompletedNodes = map.CompletedNodes.ToArray(),
            CurrentPosition = map.CurrentPosition,
            CurrentChoices = map.CurrentChoices.ToArray(),
            TotalNodeCount = map.TotalNodeCount
        };
}
