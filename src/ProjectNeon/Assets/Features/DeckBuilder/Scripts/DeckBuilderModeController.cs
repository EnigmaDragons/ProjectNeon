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
    [SerializeField] private LibraryFilterUI cardFilter;
    [SerializeField] private HeroDetailsPanelForCustomization heroDetails;

    private void Awake()
    {
        saveButton.onClick.AddListener(() =>
        {
            if (state.HeroesDecks.All(x => x.Deck.Count == deckSize))
            {
                party.UpdateDecks(state.HeroesDecks.Select(x => x.Deck).ToArray());
                Message.Publish(new AutoSaveRequested());
                parent.SetActive(false);
            }
        });
        fightButton.onClick.AddListener(() => OnFightButtonClicked());
    }
    
    protected override void Execute(TogglePartyDetails msg)
    {
        parent.SetActive(true);
        equipmentLibraryUI.GenerateLibrary();
        cardFilter.Regenerate();
        heroDetails.Initialized();
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

        var initialTab = party.Equipment.Available.Any() ? "equipment" : "hero";
        if (initialTab.Equals("equipment"))
        {
            var heroes = party.Heroes;
            var firstHeroWithAvailableEquipment = heroes.Where(h => party.Equipment.AvailableFor(h.Character).Any()).FirstAsMaybe();
            firstHeroWithAvailableEquipment.IfPresent(h => state.SelectedHeroesDeck = new HeroesDeck {Hero = h, Deck = h.Deck.Cards});
        }
        Message.Publish(new CustomizationTabSwitched { TabName = initialTab });
    }

    private void OnFightButtonClicked()
    {
        if (state.HeroesDecks.Any(x => x.Deck.Count != deckSize)) 
            return;
        
        if (party.HasAnyUnequippedGear())
            Message.Publish(new ShowTwoChoiceDialog
            {
                Prompt = "You have unequipped gear. Are you sure you're ready to fight?",
                PrimaryButtonText = "Yes",
                PrimaryAction = BeginFight,
                SecondaryButtonText = "Go Back",
                SecondaryAction = () => { },
                UseDarken = true
            });
        else
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