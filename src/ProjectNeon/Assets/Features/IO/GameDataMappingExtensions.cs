using System.Linq;

public static class GameDataMappingExtensions
{
    public static GamePartyData GetData(this PartyAdventureState s)
        => new GamePartyData
        {
            Credits = s.Credits,
            ClinicVouchers = s.ClinicVouchers,
            Heroes = s.Heroes
                .Select(h => new GameHeroData
                {
                    BaseHeroId = h.Character.Id,
                    BasicCardId = h.BasicCard.Id,
                    Health = h.Health,
                    Stats = h.LevelUpsAndImplants.GetSaveData(),
                    Levels = h.Levels,
                    Deck = new GameDeckData { CardIds = h.Deck.Cards.Select(c => c.Id).ToArray() },
                    EquipmentIdNames = h.Equipment.All.Where(e => !h.Equipment.Implants.Contains(e))
                        .Select(x => new GameEquipmentIdName { Id = x.Id, Name = x.Name }).ToArray(),
                    Implants = h.Equipment.Implants.Select(x => x.GetData()).ToArray(),
                    PrimaryStat = h.PlayerPrimaryStatSelection
                }).ToArray(),
            CardIds = s.Cards.AllCards
                .SelectMany(cSlot => Enumerable.Range(0, cSlot.Value).Select(_ => cSlot.Key.Id))
                .ToArray(),
            Equipment = s.Equipment.All.Select(x => x.GetData()).ToArray(),
            Blessings = s.Blessings
                .Select(b => new BlessingSaveData
                {
                    Name = b.Name, 
                    TargetHeroIds = b.Targets.Select(t => t.Id).ToArray()
                })
                .ToArray(),
            CorpCostModifiers = s.CorpCostModifiers
        };

    public static GameMapData GetMapData(this GameAdventureProgressType type, CurrentGameMap3 map3, CurrentMapSegmentV5 map5)
    {
        if (type == GameAdventureProgressType.V5)
            return map5.GetData();
        if (type == GameAdventureProgressType.V2)
            return map3.GetData();
        return new GameMapData();
    }
    
    public static GameMapData GetData(this CurrentMapSegmentV5 map)
        => map.CurrentMap == null
            ? new GameMapData()
            : new GameMapData
            {
                Type = GameMapDataType.V5,
                GameMapId = map.CurrentMap.id,
                CurrentNode =  map.CurrentNode.Value,
                CurrentPosition = map.PreviousPosition,
                CurrentChoices = map.CurrentChoices.ToArray(),
                CurrentNodeRngSeed = map.CurrentNodeRngSeed.Peek.OrDefault(Rng.NewSeed),
            };
    
    public static GameMapData GetData(this CurrentGameMap3 map)
        => map.CurrentMap == null 
            ? new GameMapData() 
            : new GameMapData
                {
                    Type = GameMapDataType.V3,
                    GameMapId = map.CurrentMap.id,
                    CurrentNode =  map.CurrentNode.Value,
                    CompletedNodes = map.CompletedNodes.ToArray(),
                    CurrentPosition = map.PreviousPosition,
                    CurrentChoices = map.CurrentChoices.ToArray(),
                    HasCompletedEventEnRoute = map.HasCompletedEventEnRoute,
                    CurrentNodeRngSeed = map.CurrentNodeRngSeed,
                    HeatAdjustments = map.HeatAdjustments
                };
}
