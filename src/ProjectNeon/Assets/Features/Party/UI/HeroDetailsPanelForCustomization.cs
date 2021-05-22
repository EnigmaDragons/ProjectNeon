using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroDetailsPanelForCustomization : OnMessage<HeroStateChanged, DeckBuilderHeroSelected>
{
    [SerializeField] private DeckBuilderState deckBuilderState;
    [SerializeField] private Image heroBust;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI classLabel;
    [SerializeField] private MemberStatPanel stats;
    [SerializeField] private HeroEquipmentPanelV2 equipment;
    [SerializeField] private HeroInjuryPanel injuries;
    [SerializeField] private TextCommandButton levelUpButton;
    [SerializeField] private TextMeshProUGUI levelLabel;

    public HeroDetailsPanelForCustomization Initialized()
    {
        var hero = deckBuilderState.SelectedHeroesDeck.Hero;
        nameLabel.text = hero.Name;
        classLabel.text = hero.Class;
        levelLabel.text = hero.Level.ToString();
        heroBust.sprite = hero.Character.Bust;
        stats.Initialized(hero);
        injuries.Init(hero);
        equipment.Initialized();
        
        levelUpButton?.gameObject.SetActive(false);
        if (levelUpButton != null && hero.Levels.LevelUpPoints > 0)
            levelUpButton.Init("Level Up", () => Message.Publish(new LevelUpHero(hero)));
        
        return this;
    }

    protected override void Execute(HeroStateChanged msg) => Initialized();
    protected override void Execute(DeckBuilderHeroSelected msg) => Initialized();
}