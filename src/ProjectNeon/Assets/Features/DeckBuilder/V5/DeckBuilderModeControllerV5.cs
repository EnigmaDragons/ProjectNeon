using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DeckBuilderModeControllerV5 : OnMessage<TogglePartyDetails, DeckBuilderCurrentDeckChanged>
{
    [SerializeField] private IntReference deckSize;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private GameObject parent;
    [SerializeField] private Button saveButton;
    [SerializeField] private LocalizedCommandButton saveButtonCont;
    [SerializeField] private GameObject[] fightOnlyElements;
    [SerializeField] private Button fightButton;
    [SerializeField] private LocalizedCommandButton fightButtonCont;
    [SerializeField] private Navigator navigator;

    private bool _doneButtonCannotBeInteractive;
    
    private void Start()
    {
        saveButtonCont.Init(OnFinished);
        fightButton.onClick.AddListener(OnFinished);
    }
    
    protected override void Execute(TogglePartyDetails msg)
    {
        parent.SetActive(true);
        if (msg.ShouldSaveOnFinished)
        {
            saveButton.gameObject.SetActive(true);
            fightOnlyElements.ForEach(x => x.SetActive(false));
            fightButton.gameObject.SetActive(false);
            state.OnDeckbuilderClosedAction = OnSaveButtonClicked;
        }
        else if (msg.ShouldFightOnFinished)
        {
            saveButton.gameObject.SetActive(false);
            fightOnlyElements.ForEach(x => x.SetActive(true));
            fightButton.gameObject.SetActive(true);
            state.OnDeckbuilderClosedAction = OnFightButtonClicked;
        }
        else // Is Tutorial
        {
            saveButton.gameObject.SetActive(true);
            fightOnlyElements.ForEach(x => x.SetActive(true));
            fightButton.gameObject.SetActive(false);
            state.OnDeckbuilderClosedAction = () => parent.SetActive(false);
        }

        var initialTab = "hero";
        Message.Publish(new CustomizationTabSwitched {TabName = initialTab});
        if (msg.ClearDeckOnShow)
        {
            state.HeroesDecks.ForEach(x => x.Deck.Clear());
            Message.Publish(new DeckCleared());
            Message.Publish(new DeckBuilderCurrentDeckChanged(state.SelectedHeroesDeck));
        }
    }

    private void OnFinished() => state.OnDeckbuilderClosedAction();

    private void OnSaveButtonClicked() 
    {
        if (state.HeroesDecks.All(x => x.Deck.Count == deckSize))
        {
            party.UpdateDecks(state.HeroesDecks.Select(x => x.Deck).ToArray());
            Message.Publish(new AutoSaveRequested());
            parent.SetActive(false);
        }
    }
    
    private void OnFightButtonClicked()
    {
        if (state.HeroesDecks.Any(x => x.Deck.Count != deckSize) || _doneButtonCannotBeInteractive) 
            return;
        
        BeginFight();
    }

    private void BeginFight()
    {
        fightButton.gameObject.SetActive(false);
        party.UpdateDecks(state.HeroesDecks.Select(x => x.Deck).ToArray());
        AllMetrics.PublishDecks(party.Heroes.Select(h => h.NameTerm.ToEnglish()).ToArray(), 
            party.Decks.Select(h => h.Cards.Select(c => c.Name).ToArray()).ToArray());
        Message.Publish(new StartBattleInitiated(fightButton.transform));
        Message.Publish(new AutoSaveRequested());
        navigator.NavigateToBattleScene();
    }

    protected override void Execute(DeckBuilderCurrentDeckChanged msg)
    {
        UpdateSaveButtonCont();
    }

    private void UpdateSaveButtonCont()
    {
        if (state == null || state.HeroesDecks == null || state.HeroesDecks.AnyNonAlloc(h => h.Deck == null))
            return;
        
        if (state.HeroesDecks.All(x => x.Deck.Count == deckSize) && !_doneButtonCannotBeInteractive)
        {
            saveButtonCont.SetButtonDisabled(false, Color.white);
            fightButtonCont.SetButtonDisabled(false, Color.white);
        }
        else
        {
            saveButtonCont.SetButtonDisabled(true, Color.white);
            fightButtonCont.SetButtonDisabled(true, Color.white);
        }
    }

    public void SetSaveButtonContInteractivity(bool isInteractive)
    {
        _doneButtonCannotBeInteractive = !isInteractive;
        UpdateSaveButtonCont();
    }
}
