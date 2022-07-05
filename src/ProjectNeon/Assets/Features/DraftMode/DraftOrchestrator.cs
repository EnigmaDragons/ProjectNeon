using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DraftOrchestrator : OnMessage<BeginDraft, DraftStepCompleted, SkipDraft>
{
    [SerializeField] private DraftState draftState;
    [SerializeField] private CurrentAdventure adventure;
    [SerializeField] private Library library;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private CardType[] blankDraftDeck;
    [SerializeField] private EquipmentPool gearPool;
    [SerializeField] private ShopCardPool cardPool;
    [SerializeField] private GameObject draftUi;
    
    private LootPicker _picker;

    protected override void Execute(BeginDraft msg)
    {
        draftState.Init(adventure.Adventure.PartySize);
        draftUi.SetActive(true);
        _picker = new LootPicker(1, new DefaultRarityFactors(), party);
        Advance();
    }

    protected override void Execute(DraftStepCompleted msg) => Advance();
    
    protected override void Execute(SkipDraft msg)
    {
        if (!draftState.PickedHeroes())
        {
            for (var i = 0; i < adventure.Adventure.PartySize; i++)
            {
                var currentParty = party.Party.Heroes;
                var featuredThree = PickNewHeroFrom3RandomSegment.GetFeatureHeroOptions(library, currentParty);
                var choice = featuredThree.Random();
                party.WithAddedDraftHero(choice, CreateBlankDeck());
            }
        }
        FinishDraftWithoutConfiguringDeck();
    }

    private void Advance()
    {
        var draftStep = draftState.Advance();
        if (draftStep == DraftStep.PickHero) 
            SelectHero();
        else if (draftStep == DraftStep.PickCard)
            SelectCard();
        else if (draftStep == DraftStep.PickGear)
            SelectGear();
        else if (draftStep == DraftStep.Finished)
            FinishDraft();
    }

    private void SelectGear()
    {
        var currentHero = party.Heroes[draftState.HeroIndex];
        var gearOptions = HeroPermanentAugmentOptions.GenerateHeroGearOptions(gearPool, party, currentHero.Character, new HashSet<Rarity>
        {
            Rarity.Common,
            Rarity.Uncommon,
            Rarity.Rare,
            Rarity.Epic
        }, 5);
        Message.Publish(new GetUserSelectedEquipmentForDraft(gearOptions, e =>
        {
            if (e.IsMissing)
            {
                Log.Error("Draft Gear Selection - No Gear Picked. Should Not Be Possible");
                FinishDraft();
            }

            var gear = e.Value;
            AllMetrics.PublishDraftGearSelection(gear.Name, gearOptions.Select(g => g.Name).ToArray());
            party.Add(gear);
            currentHero.Equipment.EquipPermanent(gear);
            Message.Publish(new DraftStepCompleted());
        }));
    }

    private void SelectCard()
    {
        var currentHero = party.Heroes[draftState.HeroIndex];
        var cardsYouCantHaveMoreOf = party.CardsYouCantHaveMoreOf();
        var starterCardOptions = currentHero.Character
            .StartingCards(cardPool)
            .Where(x => x.Archetypes.Any())
            .Where(x => !cardsYouCantHaveMoreOf.Contains(x.Id))
            .Distinct()
            .TakeRandom(2);
        
        var nonStarterOptions = _picker.PickCards(cardPool, currentHero.Archetypes, 5, new [] {Rarity.Common, Rarity.Uncommon, Rarity.Rare, Rarity.Epic});
        var member = currentHero.AsMember(draftState.HeroIndex);
        var options = starterCardOptions.Concat(nonStarterOptions).Select(s => new Card(NextCardId.Get(), member, s)).ToArray().Shuffled();
        Message.Publish(new GetUserSelectedCardForDraft(options, e =>
        {
            if (e.IsMissing)
            {
                Log.Error("Draft Card Selection - No Card Picked. Should Not Be Possible");
                FinishDraft();
            }

            var selected = e.Value;
            AllMetrics.PublishDraftCardSelection(selected.Name, options.Select(g => g.Name).ToArray());
            party.Cards.Add(e.Value.BaseType);
            Message.Publish(new DraftStepCompleted());
        }));
    }

    private RuntimeDeck CreateBlankDeck() => new RuntimeDeck {Cards = blankDraftDeck.Cast<CardTypeData>().ToList()};
    
    private void SelectHero()
    {
        var currentParty = party.Party.Heroes;
        var featuredThree = PickNewHeroFrom3RandomSegment.GetFeatureHeroOptions(library, currentParty);
        var prompt = currentParty.Length == 0 ? "Choose Your Mission Squad Leader" : "Choose A New Squad Member";
        Message.Publish(new GetUserSelectedHero(prompt, featuredThree, h =>
        {
            AllMetrics.PublishHeroSelected(h.Name, featuredThree.Select(x => x.Name).ToArray(), currentParty.Select(x => x.Name).ToArray());
            party.WithAddedDraftHero(h, CreateBlankDeck());
            Message.Publish(new AddHeroToPartyRequested(h));
            Async.ExecuteAfterDelay(0.5f, () =>
            {
                Message.Publish(new ToggleNamedTarget("HeroSelectionView"));
                Message.Publish(new DraftStepCompleted());
            });
        }));
    }
    
    private void FinishDraft()
    {
        draftUi.SetActive(false);
        Message.Publish(new HideNamedTarget("HeroSelectionView"));
        Message.Publish(new NodeFinished());
        Message.Publish(new TogglePartyDetails { ClearDeckOnShow = true });
    }
    
    private void FinishDraftWithoutConfiguringDeck()
    {
        draftUi.SetActive(false);
        Message.Publish(new HideNamedTarget("HeroSelectionView"));
        Message.Publish(new NodeFinished());
    }
}
