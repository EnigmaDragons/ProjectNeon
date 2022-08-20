using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DraftOverviewPresenter : OnMessage<DraftStateUpdated>
{
    [SerializeField] private DraftState draftState;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private TextMeshProUGUI stepLabel;
    [SerializeField] private DecklistUIController decklistUi;
    [SerializeField] private Button viewHeroDetailsButton;
    [SerializeField] private Image heroBust;
    [SerializeField] private TextMeshProUGUI heroName;
    [SerializeField] private GameObject currentHeroUi;
    [SerializeField] private DraftHeroButton[] heroes;
    [SerializeField] private EquipmentPresenter[] gear;
    
    private void Awake()
    {
        viewHeroDetailsButton.onClick.AddListener(ViewHeroDetails);
    }
    
    protected override void AfterEnable()
    {
        Render();
    }
    
    protected override void Execute(DraftStateUpdated msg)
    {
        stepLabel.text = $"Draft Step: {msg.StepNumber}/{msg.TotalStepsCount}";
        Render();
    }

    private bool HeroesReady => !(party.Heroes.None() || draftState.HeroIndex < 0 || draftState.HeroIndex >= party.Heroes.Length);
    
    private void Render()
    {
        var heroesReady = HeroesReady;
        viewHeroDetailsButton.gameObject.SetActive(heroesReady);
        currentHeroUi.SetActive(heroesReady);
        if (!heroesReady)
            return;
        
        RenderHeroes();
        
        var currentHero = party.Heroes[draftState.HeroIndex];
        RenderCards(currentHero);
        RenderGear(currentHero);
    }

    private void RenderGear(Hero currentHero)
    {
        var equippedGear = currentHero.Equipment.All.Where(x => x.Slot != EquipmentSlot.Permanent).ToArray();
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

            decklistUi.ShowDeckList(currentHero.BasicCard.ToNonBattleCard(currentHero).Concat(cards).ToArray());
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
        
        var currentHero = party.Heroes[draftState.HeroIndex];
        heroBust.sprite = currentHero.Character.Bust;
        heroName.text = currentHero.Name;
    }
    
    private void ViewHeroDetails()
    {
        if (!HeroesReady)
            return;

        var currentHero = party.Heroes[draftState.HeroIndex];
        Message.Publish(new ShowHeroDetailsView(currentHero));
    }
}
