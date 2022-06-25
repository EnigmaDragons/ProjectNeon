using System.Linq;
using UnityEngine;

public class DraftOrchestrator : OnMessage<BeginDraft, DraftStepCompleted>
{
    [SerializeField] private CurrentAdventure adventure;
    [SerializeField] private Library library;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private CardType[] blankDraftDeck;

    private int _numDraftHeroes = 0;
    private int _heroIndex = -1;
    private int _draftStepIndex = 0;

    private DraftStep[] DraftSteps = {
        DraftStep.PickHero,
        DraftStep.PickGear,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickGear,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
    };
    
    private enum DraftStep
    {
        PickHero = 0,
        PickGear = 1,
        PickCard = 2,
    }

    protected override void Execute(BeginDraft msg)
    {
        _numDraftHeroes = adventure.Adventure.PartySize;
        _heroIndex = -1;
        _draftStepIndex = 0;
        Advance();
    }

    protected override void Execute(DraftStepCompleted msg) => Advance();

    private void Advance()
    {
        _heroIndex++;
        if (_heroIndex == _numDraftHeroes)
        {
            _heroIndex = 0;
            _draftStepIndex++;
        }
        if (_draftStepIndex >= DraftSteps.Length)
        {
            Message.Publish(new NodeFinished());
            return;
        }

        var draftStep = DraftSteps[_draftStepIndex];
        if (draftStep == DraftStep.PickHero) 
            SelectHero();
        else if (draftStep == DraftStep.PickCard)
            SelectCard();
        else if (draftStep == DraftStep.PickGear)
            SelectGear();
    }

    private void SelectGear()
    {
        Message.Publish(new NodeFinished());
    }

    private void SelectCard()
    {
        Message.Publish(new NodeFinished());
    }

    private void SelectHero()
    {
        var currentParty = party.Party.Heroes;
        var featuredThree = PickNewHeroFrom3RandomSegment.GetFeatureHeroOptions(library, currentParty);
        var prompt = currentParty.Length == 0 ? "Choose Your Mission Squad Leader" : "Choose A New Squad Member";
        Message.Publish(new GetUserSelectedHero(prompt, featuredThree, h =>
        {
            AllMetrics.PublishHeroSelected(h.Name, featuredThree.Select(x => x.Name).ToArray(), currentParty.Select(x => x.Name).ToArray());
            party.WithAddedDraftHero(h, new RuntimeDeck { Cards = blankDraftDeck.Cast<CardTypeData>().ToList() });
            Message.Publish(new AddHeroToPartyRequested(h));
            Async.ExecuteAfterDelay(0.5f, () =>
            {
                Message.Publish(new ToggleNamedTarget("HeroSelectionView"));
                Message.Publish(new DraftStepCompleted());
            });
        }));
    }
}
