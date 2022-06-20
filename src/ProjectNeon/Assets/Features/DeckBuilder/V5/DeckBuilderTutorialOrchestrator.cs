using UnityEngine;
using UnityEngine.UI;

public class DeckBuilderTutorialOrchestrator : MonoBehaviour
{
    [SerializeField] private BaseHero tutorialHero;
    
    [SerializeField] private GameObject enemyTabHighlight;
    [SerializeField] private GameObject deckTabHighlight;
    [SerializeField] private GameObject deckClearHighlight;
    [SerializeField] private GameObject heroStatsHighlight;
    [SerializeField] private GameObject libraryNextHighlight;
    [SerializeField] private Button libraryNextButton;
    [SerializeField] private DeckBuilderModeControllerV5 deckBuilderModeControllerV5;

    private bool _hasSwitchedToEnemyTab;
    private bool _hasSwitchedToHeroTab;
    private bool _hasSwitchedHeroes;
    private bool _hasClearedDeck;
    private bool _hasViewedHeroStats;
    private bool _hasAddedCard;
    private bool _hasRemovedCard;
    private bool _hasChangedLibraryPages;
    
    private void OnEnable()
    {
        if (CurrentAcademyData.Data.IsLicensedBenefactor)
            return;
        Message.Subscribe<CustomizationTabSwitched>(Execute, this);
        Message.Subscribe<DeckBuilderHeroSelected>(Execute, this);
        Message.Subscribe<DeckCleared>(Execute, this);
        Message.Subscribe<ShowHeroDetailsView>(Execute, this);
        Message.Subscribe<CardRemovedFromDeck>(Execute, this);
        Message.Subscribe<CardAddedToDeck>(Execute, this);
        enemyTabHighlight.SetActive(true);
        Message.Publish(new SetSuperFocusDeckBuilderControl(DeckBuilderControls.HeroTab, true));
        deckClearHighlight.SetActive(true);
        heroStatsHighlight.SetActive(true);
        Message.Publish(new SetSuperFocusDeckBuilderControl(DeckBuilderControls.CardInLibrary, true));
        Message.Publish(new SetSuperFocusDeckBuilderControl(DeckBuilderControls.CardInDeck, true));
        libraryNextHighlight.SetActive(true);
        libraryNextButton.onClick.AddListener(() =>
        {
            if (!_hasChangedLibraryPages)
            {
                _hasChangedLibraryPages = true;
                libraryNextHighlight.SetActive(false);
            }
        });
        SetDoneButtonInteractivity();
    }
        
    private void OnDisable() => Message.Unsubscribe(this);

    private void Execute(CustomizationTabSwitched msg)
    {
        if (msg.TabName == "enemy" && !_hasSwitchedToEnemyTab)
        {
            _hasSwitchedToEnemyTab = true;
            enemyTabHighlight.SetActive(false);
            deckTabHighlight.SetActive(true);
            SetDoneButtonInteractivity();
        }
        if (msg.TabName == "hero" && _hasSwitchedToEnemyTab && !_hasSwitchedToHeroTab)
        {
            _hasSwitchedToHeroTab = true;
            deckTabHighlight.SetActive(false);
            SetDoneButtonInteractivity();
        }
    }

    private void Execute(DeckBuilderHeroSelected msg)
    {
        if (msg.HeroesDeck.Hero.Name != tutorialHero.Name && !_hasSwitchedHeroes)
        {
            _hasSwitchedHeroes = true;
            Message.Publish(new SetSuperFocusDeckBuilderControl(DeckBuilderControls.HeroTab, false));
            SetDoneButtonInteractivity();
        }
    }

    private void Execute(DeckCleared msg)
    {
        if (!_hasClearedDeck)
        {
            _hasClearedDeck = true;
            deckClearHighlight.SetActive(false);
            SetDoneButtonInteractivity();
        }
    }
    
    private void Execute(ShowHeroDetailsView msg)
    {
        if (!_hasViewedHeroStats)
        {
            _hasViewedHeroStats = true;
            heroStatsHighlight.SetActive(false);
            SetDoneButtonInteractivity();
        }
    }
    
    private void Execute(CardRemovedFromDeck msg)
    {
        if (!_hasRemovedCard)
        {
            _hasRemovedCard = true;
            Message.Publish(new SetSuperFocusDeckBuilderControl(DeckBuilderControls.CardInDeck, false));
            SetDoneButtonInteractivity();
        }
    }
    
    private void Execute(CardAddedToDeck msg)
    {
        if (!_hasAddedCard)
        {
            _hasAddedCard = true;
            Message.Publish(new SetSuperFocusDeckBuilderControl(DeckBuilderControls.CardInLibrary, false));
            SetDoneButtonInteractivity();
        }
    }

    private void SetDoneButtonInteractivity() => deckBuilderModeControllerV5.SetSaveButtonContInteractivity(
           _hasSwitchedToEnemyTab 
        && _hasSwitchedToHeroTab
        && _hasSwitchedHeroes
        && _hasClearedDeck
        && _hasViewedHeroStats
        && _hasAddedCard
        && _hasRemovedCard);
}