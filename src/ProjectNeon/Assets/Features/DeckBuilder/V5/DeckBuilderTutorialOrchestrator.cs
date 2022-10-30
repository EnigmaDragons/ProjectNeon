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
        Message.Subscribe<ShowHeroDetailsView>(Execute, this);
        Message.Subscribe<CardRemovedFromDeck>(Execute, this);
        Message.Subscribe<CardAddedToDeck>(Execute, this);
        
        SetFocusEnemyTabActive(true);
        SetFocusDeckTabActive(false);
        SetFocusHeroStatsActive(false);
        SetFocusHeroTabActive(false);
        SetFocusAddCardActive(false);
        SetFocusRemoveCardActive(false);
        SetFocusLibraryNextActive(false);
        
        libraryNextButton.onClick.AddListener(() =>
        {
            if (!_hasChangedLibraryPages)
            {
                _hasChangedLibraryPages = true;
                SetNextFocus();
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
            SetNextFocus();
        }
        if (msg.TabName == "hero" && _hasSwitchedToEnemyTab && !_hasSwitchedToHeroTab)
        {
            _hasSwitchedToHeroTab = true;
            SetNextFocus();
        }
    }

    private void Execute(DeckBuilderHeroSelected msg)
    {
        if (msg.HeroesDeck.Hero.NameTerm != tutorialHero.NameTerm() && !_hasSwitchedHeroes)
        {
            _hasSwitchedHeroes = true;
            SetNextFocus();
        }
    }

    private void Execute(ShowHeroDetailsView msg)
    {
        if (!_hasViewedHeroStats)
        {
            _hasViewedHeroStats = true;
            SetNextFocus();
        }
    }
    
    private void Execute(CardRemovedFromDeck msg)
    {
        if (!_hasRemovedCard)
        {
            _hasRemovedCard = true;
            SetNextFocus();
        }
    }
    
    private void Execute(CardAddedToDeck msg)
    {
        if (!_hasAddedCard)
        {
            _hasAddedCard = true;
            SetNextFocus();
        }
    }

    private void SetNextFocus()
    {
        var hasFocused = false;
        SetFocusEnemyTabActive(!_hasSwitchedToEnemyTab);
        hasFocused = !_hasSwitchedToEnemyTab;
        SetFocusDeckTabActive(!_hasSwitchedToHeroTab && !hasFocused);
        hasFocused = !_hasSwitchedToHeroTab || hasFocused;
        SetFocusHeroStatsActive(!_hasViewedHeroStats && !hasFocused);
        hasFocused = !_hasViewedHeroStats || hasFocused;
        SetFocusHeroTabActive(!_hasSwitchedHeroes && !hasFocused);
        hasFocused = !_hasSwitchedHeroes || hasFocused;
        SetFocusAddCardActive(!_hasAddedCard && !hasFocused);
        hasFocused = !_hasAddedCard || hasFocused;
        SetFocusRemoveCardActive(!_hasRemovedCard && !hasFocused);
        hasFocused = !_hasRemovedCard || hasFocused;
        SetFocusLibraryNextActive(!_hasChangedLibraryPages && !hasFocused);
        SetDoneButtonInteractivity();
    }
    
    private void SetFocusEnemyTabActive(bool isActive) => enemyTabHighlight.SetActive(isActive);
    private void SetFocusDeckTabActive(bool isActive) => deckTabHighlight.SetActive(isActive);
    private void SetFocusHeroStatsActive(bool isActive) => heroStatsHighlight.SetActive(isActive);
    private void SetFocusHeroTabActive(bool isActive) => Message.Publish(new SetSuperFocusDeckBuilderControl(DeckBuilderControls.HeroTab, isActive));
    private void SetFocusAddCardActive(bool isActive) => Message.Publish(new SetSuperFocusDeckBuilderControl(DeckBuilderControls.CardInLibrary, isActive));
    private void SetFocusRemoveCardActive(bool isActive) => Message.Publish(new SetSuperFocusDeckBuilderControl(DeckBuilderControls.CardInDeck, isActive));
    private void SetFocusLibraryNextActive(bool isActive) => libraryNextHighlight.SetActive(isActive);

    private void SetDoneButtonInteractivity() => deckBuilderModeControllerV5.SetSaveButtonContInteractivity(
        _hasSwitchedToEnemyTab 
        && _hasSwitchedToHeroTab
        && _hasSwitchedHeroes
        && _hasViewedHeroStats
        && _hasAddedCard
        && _hasRemovedCard);
}