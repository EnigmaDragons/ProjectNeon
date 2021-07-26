using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroDetailsPanelForCustomization : OnMessage<HeroStateChanged, DeckBuilderHeroSelected>
{
    [SerializeField] private DeckBuilderState deckBuilderState;
    [SerializeField] private Image heroBust;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI classLabel;
    [SerializeField] private MemberResourcePanel resources;
    [SerializeField] private MemberStatPanel stats;
    [SerializeField] private HeroEquipmentPanelV2 equipment;
    [SerializeField] private HeroInjuryPanel injuries;
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private TMP_Dropdown primaryStat;

    private bool _ignoreChanges;
    private Hero _hero;
    private Dictionary<int, StatType> _optionToStat;

    private void Awake() => primaryStat.onValueChanged.AddListener(x =>
    {
        if (!_ignoreChanges)
            _hero.SetPrimaryStat(_optionToStat[x]);
    }); 
    
    public HeroDetailsPanelForCustomization Initialized()
    {
        _ignoreChanges = true;
        _hero = deckBuilderState.SelectedHeroesDeck.Hero;
        nameLabel.text = _hero.Name;
        classLabel.text = _hero.Class;
        levelLabel.text = _hero.Level.ToString();
        heroBust.sprite = _hero.Character.Bust;
        _optionToStat = new Dictionary<int, StatType>();
        var options = StatsExtensions.PrimaryStatOptions.Where(x => _hero.PrimaryStat == x || _hero.Character.Stats[x] > 0).ToArray();
        for (var i = 0; i < options.Length; i++)
            _optionToStat[i] = options[i];
        primaryStat.options = options.Select(x => new TMP_Dropdown.OptionData(x.ToString())).ToList();
        primaryStat.value = _optionToStat.First(x => x.Value == _hero.PrimaryStat).Key;
        resources.Initialized(_hero);
        stats.Initialized(_hero);
        injuries.Init(_hero);
        equipment.Initialized();
        _ignoreChanges = false;
        return this;
    }

    protected override void Execute(HeroStateChanged msg) => Initialized();
    protected override void Execute(DeckBuilderHeroSelected msg) => Initialized();
}