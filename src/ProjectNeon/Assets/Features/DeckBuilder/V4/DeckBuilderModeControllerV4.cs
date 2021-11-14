using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DeckBuilderModeControllerV4 : OnMessage<TogglePartyDetails, DeckBuilderCurrentDeckChanged>
{
    [SerializeField] private IntReference deckSize;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private GameObject parent;
    [SerializeField] private Button saveButton;
    [SerializeField] private TextCommandButton saveButtonCont;
    [SerializeField] private GameObject[] fightOnlyElements;
    [SerializeField] private Button fightButton;
    [SerializeField] private Navigator navigator;
    [SerializeField] private LibraryFilterUI cardFilter;

    private void Awake()
    {
        saveButtonCont.Init("Save", () =>
        {
            if (state.HeroesDecks.All(x => x.Deck.Count == deckSize))
            {
                party.UpdateDecks(state.HeroesDecks.Select(x => x.Deck).ToArray());
                Message.Publish(new AutoSaveRequested());
                parent.SetActive(false);
            }
        });
        fightButton.onClick.AddListener(OnFightButtonClicked);
    }
    
    protected override void Execute(TogglePartyDetails msg)
    {
        parent.SetActive(true);
        cardFilter.Regenerate();
        if (msg.AllowDone)
        {
            saveButton.gameObject.SetActive(true);
            fightOnlyElements.ForEach(x => x.SetActive(false));
            fightButton.gameObject.SetActive(false);
        }
        else
        {
            saveButton.gameObject.SetActive(false);
            fightOnlyElements.ForEach(x => x.SetActive(true));
            fightButton.gameObject.SetActive(true);
        }

        var initialTab = "hero";
        Message.Publish(new CustomizationTabSwitched { TabName = initialTab });
    }

    private void OnFightButtonClicked()
    {
        if (state.HeroesDecks.Any(x => x.Deck.Count != deckSize)) 
            return;
        
        BeginFight();
    }

    private void BeginFight()
    {
        fightButton.gameObject.SetActive(false);
        party.UpdateDecks(state.HeroesDecks.Select(x => x.Deck).ToArray());
        Message.Publish(new StartBattleInitiated(fightButton.transform));
        Message.Publish(new AutoSaveRequested());
        navigator.NavigateToBattleScene();
    }

    protected override void Execute(DeckBuilderCurrentDeckChanged msg)
    {
        if (state.HeroesDecks.All(x => x.Deck.Count == deckSize))
        {
            saveButtonCont.SetButtonDisabled(false, Color.white);
            saveButtonCont.SetButtonDisabled(false, Color.white);
        }
        else
        {
            saveButtonCont.SetButtonDisabled(true, Color.white);
            saveButtonCont.SetButtonDisabled(true, Color.white);
        }
    }
}
