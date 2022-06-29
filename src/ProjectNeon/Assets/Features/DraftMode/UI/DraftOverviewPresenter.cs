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
    [SerializeField] private DraftHeroButton[] heroes;
    [SerializeField] private EquipmentPresenter[] gear;

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
        
        RenderHeroes();
        
        var currentHero = party.Heroes[draftState.HeroIndex];
        RenderCards(currentHero);
        RenderGear(currentHero);
    }

    private void RenderGear(Hero currentHero)
    {
        var equippedGear = currentHero.Equipment.All;
        for (var i = 0; i < gear.Length; i++)
        {
            var hasEquipment = equippedGear.Length > i;
            if (hasEquipment)
                gear[i].Initialized(equippedGear[i], () => { }, false, false);
            else
                gear[i].gameObject.SetActive(false);
        }
    }

    private void RenderCards(Hero currentHero)
    {
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

    private void RenderHeroes()
    {
        for (var i = 0; i < heroes.Length; i++)
        {
            var hero = heroes[i];
            if (party.Heroes.Length > i)
                hero.Init(party.Heroes[i].Character, draftState.HeroIndex == i);
            else
                hero.Disable();
        }
    }
}
