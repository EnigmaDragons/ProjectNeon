using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DeckBuilderModeController : OnMessage<TogglePartyDetails, DeckBuilderCurrentDeckChanged>
{
    [SerializeField] private IntReference deckSize;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private GameObject parent;
    [SerializeField] private Button saveButton;
    [SerializeField] private TextCommandButton saveButtonCont;
    [SerializeField] private GameObject[] fightOnlyElements;
    [SerializeField] private Button fightButton;
    [SerializeField] private TextCommandButton fightButtonCont;
    [SerializeField] private Navigator navigator;
    [SerializeField] private EquipmentLibraryUI equipmentLibraryUI;

    private void Awake()
    {
        saveButton.onClick.AddListener(() =>
        {
            if (state.HeroesDecks.All(x => x.Deck.Count == deckSize))
            {
                party.UpdateDecks(state.HeroesDecks.Select(x => x.Deck).ToArray());
                parent.SetActive(false);   
            }
        });
        fightButton.onClick.AddListener(() =>
        {
            if (state.HeroesDecks.All(x => x.Deck.Count == deckSize))
            {
                party.UpdateDecks(state.HeroesDecks.Select(x => x.Deck).ToArray());
                navigator.NavigateToBattleScene();
            }
        });
    }
    
    protected override void Execute(TogglePartyDetails msg)
    {
        parent.SetActive(true);
        equipmentLibraryUI.GenerateLibrary();
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