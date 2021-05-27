using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LibraryFilterUI : OnMessage<DeckBuilderHeroSelected, CustomizationTabSwitched>
{
    [SerializeField] private Toggle showFormulas;
    [SerializeField] private Toggle starterToggle;
    [SerializeField] private Toggle commonToggle;
    [SerializeField] private Toggle uncommonToggle;
    [SerializeField] private Toggle rareToggle;
    [SerializeField] private Toggle epicToggle;
    [SerializeField] private GameObject archetypeParent;
    [SerializeField] private ArchetypeToggle archetypeToggle;
    [SerializeField] private DeckBuilderState deckBuilderState;

    private HashSet<string> _selectedArchetypes;
    
    protected override void Execute(DeckBuilderHeroSelected msg) => Regenerate();
    protected override void Execute(CustomizationTabSwitched msg) => Regenerate();
    
    private void Awake()
    {
        showFormulas.onValueChanged.AddListener(x => UpdateFilters());
        starterToggle.onValueChanged.AddListener(x => UpdateFilters());
        commonToggle.onValueChanged.AddListener(x => UpdateFilters());
        uncommonToggle.onValueChanged.AddListener(x => UpdateFilters());
        rareToggle.onValueChanged.AddListener(x => UpdateFilters());
        epicToggle.onValueChanged.AddListener(x => UpdateFilters());
    }
    
    public void Regenerate()
    {
        showFormulas.SetIsOnWithoutNotify(deckBuilderState.ShowFormulas);
        starterToggle.SetIsOnWithoutNotify(false);
        commonToggle.SetIsOnWithoutNotify(false);
        uncommonToggle.SetIsOnWithoutNotify(false);
        rareToggle.SetIsOnWithoutNotify(false);
        epicToggle.SetIsOnWithoutNotify(false);
        _selectedArchetypes = new HashSet<string>();
        foreach (Transform child in archetypeParent.transform)
            Destroy(child.gameObject);
        Instantiate(archetypeToggle, archetypeParent.transform).Init("", x =>
        {
            if (x)
                _selectedArchetypes.Add("");
            else
                _selectedArchetypes.Remove("");
            UpdateFilters();
        });
        foreach (var archetype in deckBuilderState.SelectedHeroesDeck.Hero.Character.Archetypes)
            Instantiate(archetypeToggle, archetypeParent.transform).Init(archetype, x =>
            {
                if (x)
                    _selectedArchetypes.Add(archetype);
                else
                    _selectedArchetypes.Remove(archetype);
                UpdateFilters();
            });
        UpdateFilters();
    }

    private void UpdateFilters()
    {
        deckBuilderState.ShowFormulas = showFormulas.isOn;
        var rarities = new List<Rarity>();
        if (starterToggle.isOn)
        {
            rarities.Add(Rarity.Starter);
            rarities.Add(Rarity.Basic);
        }
        if (commonToggle.isOn)
            rarities.Add(Rarity.Common);
        if (uncommonToggle.isOn)
            rarities.Add(Rarity.Uncommon);
        if (rareToggle.isOn)
            rarities.Add(Rarity.Rare);
        if (epicToggle.isOn)
            rarities.Add(Rarity.Epic);
        deckBuilderState.ShowRarities = rarities.ToArray();
        deckBuilderState.ShowArchetypes = _selectedArchetypes.ToArray();
        deckBuilderState.ShowEquipmentSlots = new EquipmentSlot[0];
        Message.Publish(new DeckBuilderFiltersChanged());
    }
}