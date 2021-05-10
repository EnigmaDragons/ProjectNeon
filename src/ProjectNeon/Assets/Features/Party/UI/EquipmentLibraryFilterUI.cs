using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentLibraryFilterUI : OnMessage<DeckBuilderHeroSelected, CustomizationTabSwitched>
{
    [SerializeField] private Toggle starterToggle;
    [SerializeField] private Toggle commonToggle;
    [SerializeField] private Toggle uncommonToggle;
    [SerializeField] private Toggle rareToggle;
    [SerializeField] private Toggle epicToggle;
    [SerializeField] private Toggle weaponToggle;
    [SerializeField] private Toggle armorToggle;
    [SerializeField] private Toggle augmentToggle;
    [SerializeField] private DeckBuilderState deckBuilderState;
    
    protected override void Execute(DeckBuilderHeroSelected msg) => Regenerate();
    protected override void Execute(CustomizationTabSwitched msg) => Regenerate();
    
    private void Awake()
    {
        starterToggle.onValueChanged.AddListener(x => UpdateFilters());
        commonToggle.onValueChanged.AddListener(x => UpdateFilters());
        uncommonToggle.onValueChanged.AddListener(x => UpdateFilters());
        rareToggle.onValueChanged.AddListener(x => UpdateFilters());
        epicToggle.onValueChanged.AddListener(x => UpdateFilters());
        weaponToggle.onValueChanged.AddListener(x => UpdateFilters());
        armorToggle.onValueChanged.AddListener(x => UpdateFilters());
        augmentToggle.onValueChanged.AddListener(x => UpdateFilters());
    }
    
    public void Regenerate()
    {
        starterToggle.SetIsOnWithoutNotify(false);
        commonToggle.SetIsOnWithoutNotify(false);
        uncommonToggle.SetIsOnWithoutNotify(false);
        rareToggle.SetIsOnWithoutNotify(false);
        epicToggle.SetIsOnWithoutNotify(false);
        weaponToggle.SetIsOnWithoutNotify(false);
        armorToggle.SetIsOnWithoutNotify(false);
        augmentToggle.SetIsOnWithoutNotify(false);
        UpdateFilters();
    }
    
    private void UpdateFilters()
    {
        deckBuilderState.ShowFormulas = false;
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
        deckBuilderState.ShowArchetypes = new string[0];
        var equipmentSlots = new List<EquipmentSlot>();
        if (weaponToggle.isOn)
            equipmentSlots.Add(EquipmentSlot.Weapon);
        if (armorToggle.isOn)
            equipmentSlots.Add(EquipmentSlot.Armor);
        if (augmentToggle.isOn)
            equipmentSlots.Add(EquipmentSlot.Augmentation);
        deckBuilderState.ShowEquipmentSlots = equipmentSlots.ToArray();
        Message.Publish(new DeckBuilderFiltersChanged());
    }
}