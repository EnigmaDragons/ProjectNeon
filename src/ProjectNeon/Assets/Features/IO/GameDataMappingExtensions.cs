﻿using System.Linq;

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
                    Levels = h.Levels,
                    Deck = new GameDeckData { CardIds = h.Deck.Cards.Select(c => c.Id).ToArray() },
                    EquipmentIdNames = h.Equipment.All.Select(x => new GameEquipmentIdName { Id = x.Id, Name = x.Name }).ToArray()
                }).ToArray(),
            CardIds = s.Cards.AllCards
                .SelectMany(cSlot => Enumerable.Range(0, cSlot.Value).Select(_ => cSlot.Key.Id))
                .ToArray(),
            Equipment = s.Equipment.All.Select(x => x.GetData()).ToArray()
        };
}
