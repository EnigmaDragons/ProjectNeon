using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DraftOrchestrator : OnMessage<BeginDraft, DraftStepCompleted, SkipDraft>, ILocalizeTerms
{
    [SerializeField] private DraftState draftState;
    [SerializeField] private CurrentAdventure adventure;
    [SerializeField] private Library library;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private CardType[] blankDraftDeck;
    [SerializeField] private EquipmentPool gearPool;
    [SerializeField] private ShopCardPool cardPool;
    [SerializeField] private GameObject draftUi;
    [SerializeField] private DeterminedNodeInfo nodeInfo;
    
    private LootPicker _picker;

    protected override void Execute(BeginDraft msg)
    {
        draftUi.SetActive(true);
        _picker = new LootPicker(1, new DefaultRarityFactors(), party, new DeterministicRng(Rng.NewSeed()));
        ResolveDraftStep(draftState.Current);
    }

    protected override void Execute(DraftStepCompleted msg) => Advance();
    
    protected override void Execute(SkipDraft msg)
    {
        if (!draftState.PickedHeroes())
        {
            for (var i = 0; i < adventure.Adventure.PartySize; i++)
            {
                var currentParty = party.Party.Heroes;
                var featuredThree = PickNewHeroFrom3RandomSegment.GetFeatureHeroOptions(library, currentParty, adventure.Adventure);
                var choice = featuredThree.Random();
                party.WithAddedDraftHero(choice, CreateBlankDeck());
            }
        }
        FinishDraftWithoutConfiguringDeck();
    }

    private void Advance()
    {
        Message.Publish(new AutoSaveRequested());
        ResolveDraftStep(draftState.Advance());
    }

    private void ResolveDraftStep(DraftStep draftStep)
    {
        if (draftStep == DraftStep.PickHero)
            if (party.Party.Heroes.Length > draftState.HeroIndex)
                Advance();
            else
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
        if (nodeInfo.DraftGearSelection.IsMissing)
        {
            nodeInfo.DraftGearSelection = GearPackGeneration.GetDualRarityAugmentPack(currentHero, gearPool, party);
            Message.Publish(new SaveDeterminationsRequested());
        }
        var gearOptions = nodeInfo.DraftGearSelection.Value;
        Message.Publish(new GetUserSelectedEquipmentForDraft(gearOptions, e =>
        {
            if (e.IsMissing)
            {
                Log.Error("Draft Gear Selection - No Gear Picked. Should Not Be Possible");
                FinishDraft();
            }

            nodeInfo.DraftGearSelection = Maybe<StaticEquipment[]>.Missing();
            var gear = e.Value;
            AllMetrics.PublishDraftGearSelection(gear.Name, gearOptions.Select(g => g.Name).ToArray());
            party.Add(gear);
            currentHero.EquipPermanent(gear);
            Message.Publish(new DraftStepCompleted());
        }));
    }

    private void SelectCard()
    {
        var currentHero = party.Heroes[draftState.HeroIndex];
        
        if (nodeInfo.DraftCardSelection.IsMissing)
        {
            var cardsYouCantHaveMoreOf = party.CardsYouCantHaveMoreOf();
            var starterCardOptions = currentHero.Character
                .StartingCards(cardPool)
                .Where(x => x.Archetypes.Any())
                .Where(x => !cardsYouCantHaveMoreOf.Contains(x.Id))
                .Distinct()
                .TakeRandom(2);
            var nonStarterOptions = _picker.PickCardsForSingleHero(cardPool, currentHero.Archetypes, 5, new [] {Rarity.Common, Rarity.Uncommon, Rarity.Rare, Rarity.Epic});
            nodeInfo.DraftCardSelection = starterCardOptions.Concat(nonStarterOptions).ToArray();
            Message.Publish(new SaveDeterminationsRequested());
        }
        
        var member = currentHero.AsMember(draftState.HeroIndex);
        var options = nodeInfo.DraftCardSelection.Value.Select(s => new Card(NextCardId.Get(), member, s)).ToArray().Shuffled();
        Message.Publish(new GetUserSelectedCardForDraft(options, e =>
        {
            if (e.IsMissing)
            {
                Log.Error("Draft Card Selection - No Card Picked. Should Not Be Possible");
                FinishDraft();
            }

            nodeInfo.DraftCardSelection = Maybe<CardType[]>.Missing();
            var selected = e.Value;
            AllMetrics.PublishDraftCardSelection(selected.Name, options.Select(g => g.Name).ToArray());
            var card = e.Value.CardTypeOrNothing;
            if (card.Value)
                party.Cards.Add(card.Value);
            else
                Log.NonCrashingError("Card wasn't present in Draft Orchestrator");
            Message.Publish(new DraftStepCompleted());
        }));
    }

    private RuntimeDeck CreateBlankDeck() => new RuntimeDeck {Cards = new List<CardType>()};
    
    private void SelectHero()
    {
        var currentParty = party.Party.Heroes;
        var featuredThree = PickNewHeroFrom3RandomSegment.GetFeatureHeroOptions(library, currentParty, adventure.Adventure);
        var prompt = currentParty.Length == 0 ? "Menu/ChooseLeader" : "Menu/ChooseMember";
        Message.Publish(new GetUserSelectedHero(prompt, featuredThree, h =>
        {
            AllMetrics.PublishHeroSelected(h.NameTerm().ToEnglish(), featuredThree.Select(x => x.NameTerm().ToEnglish()).ToArray(), currentParty.Select(x => x.NameTerm().ToEnglish()).ToArray());
            party.WithAddedDraftHero(h, CreateBlankDeck());
            Message.Publish(new AddHeroToPartyRequested(h));
            Async.ExecuteAfterDelay(0.5f, () =>
            {
                Message.Publish(new ToggleNamedTarget("HeroSelectionView"));
                Message.Publish(new DraftStepCompleted());
            });
        }));
        if (party.Party.Heroes.Length >= draftState.HeroIndex && party.Party.Heroes.All(h => h.Sex == CharacterSex.Female))
            Achievements.Record(Achievement.MiscGirlPower);
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
        Message.Publish(new HideNamedTarget("DraftGearPicker"));
        Message.Publish(new HideNamedTarget("DraftCardPicker"));
        Message.Publish(new NodeFinished());
    }

    public string[] GetLocalizeTerms()
        => new[] { "Menu/ChooseLeader", "Menu/ChooseMember" };
}
