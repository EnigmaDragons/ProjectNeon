using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DraftOverviewPresenter : OnMessage<DraftStateUpdated>
{
    [SerializeField] private DraftState draftState;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private TextMeshProUGUI stepLabel;
    [SerializeField] private DecklistUIController decklistUi;

    protected override void AfterEnable()
    {
        Render();
    }
    
    protected override void Execute(DraftStateUpdated msg)
    {
        stepLabel.text = $"Draft Progress: {msg.StepNumber}/{msg.TotalStepsCount}";
        Render();
    }

    private void Render()
    {
        if (party.Heroes.None() || draftState.HeroIndex < 0 || draftState.HeroIndex >= party.Heroes.Length)
            return;
        
        var currentHero = party.Heroes[draftState.HeroIndex];

        if (decklistUi != null)
        {
            var cardsForHero = party.Cards.AllCards
                .Where(cardWithCount =>
                    cardWithCount.Key.Archetypes.All(archetype => currentHero.Character.Archetypes.Contains(archetype))
                    && cardWithCount.Key.Id != currentHero.BasicCard.Id)
                .OrderBy(c => c.Key.Archetypes.None() ? 999 : 0)
                .ThenByDescending(c => (int) c.Key.Rarity)
                .ToList();
            cardsForHero.Insert(0, new KeyValuePair<CardTypeData, int>(currentHero.BasicCard, 1));

            var cards = cardsForHero
                .SelectMany(x => Enumerable.Range(0, x.Value).Select(i => x.Key))
                .Where(x => x.Archetypes.Any())
                .Select(x => x.ToNonBattleCard(currentHero))
                .GroupBy(x => x.Name)
                .OrderBy(x => x.First().Cost.CostSortOrder())
                .ThenBy(x => x.First().Rarity)
                .ThenBy(x => x.Key)
                .SelectMany(x => x)
                .ToArray();

            decklistUi.ShowDeckList(cards);
        }
    }
}
