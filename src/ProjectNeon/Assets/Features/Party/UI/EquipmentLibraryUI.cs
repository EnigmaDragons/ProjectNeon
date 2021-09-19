using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EquipmentLibraryUI : OnMessage<EquipmentPickerCurrentGearChanged, DeckBuilderFiltersChanged>
{
    [SerializeField] private PageViewer pageViewer;
    [SerializeField] private EquipmentInLibraryButton equipmentInLibraryButtonTemplate;
    [SerializeField] private GameObject emptyEquipment;
    [SerializeField] private PartyAdventureState partyState;
    [SerializeField] private DeckBuilderState builderState;
    [SerializeField] private GameObject noEquipmentMessage;
    
    private Hero _selectedHero;
    
    protected override void Execute(EquipmentPickerCurrentGearChanged msg) => GenerateLibrary();
    protected override void Execute(DeckBuilderFiltersChanged msg) => GenerateLibrary();
    
    public void GenerateLibrary()
    {
        _selectedHero = builderState.SelectedHero;
        
        var totalCount = new Dictionary<string, int>();
        var availableCount = new Dictionary<string, int>();
        var equipUsage = new List<(Equipment, bool)>();
        
        partyState.Equipment.Available
            .Where(e => e.Archetypes.All(_selectedHero.Archetypes.Contains)
                && (builderState.ShowRarities.None() || builderState.ShowRarities.Contains(e.Rarity))
                && (builderState.ShowEquipmentSlots.None() || builderState.ShowEquipmentSlots.Contains(e.Slot)))
            .ForEach(x =>
            {
                equipUsage.Add((x, x.Slot != EquipmentSlot.Augmentation || _selectedHero.Equipment.Augments.Length != 3));
                if (!totalCount.ContainsKey(x.Name))
                    totalCount[x.Name] = 0;
                totalCount[x.Name]++;
                if (!availableCount.ContainsKey(x.Name))
                    availableCount[x.Name] = 0;
                availableCount[x.Name]++;
            });
        
        partyState.Equipment.Equipped
            .Where(e => e.Archetypes.All(_selectedHero.Archetypes.Contains)
                && (builderState.ShowRarities.None() || builderState.ShowRarities.Contains(e.Rarity))
                && (builderState.ShowEquipmentSlots.None() || builderState.ShowEquipmentSlots.Contains(e.Slot)))
            .ForEach(x =>
            {
                equipUsage.Add((x, false));
                if (!totalCount.ContainsKey(x.Name))
                    totalCount[x.Name] = 0;
                totalCount[x.Name]++;
                if (!availableCount.ContainsKey(x.Name))
                    availableCount[x.Name] = 0;
            });
        
        noEquipmentMessage.SetActive(!partyState.Equipment.All.Any());
        pageViewer.Init(
            equipmentInLibraryButtonTemplate.gameObject,
            emptyEquipment,
            equipUsage
                .GroupBy(x => x.Item1.Name).Select(x => x.First()) // Only one per button per Named Equipment type, no matter how many copies. 
                .OrderByDescending(x => x.Item1.Rarity)
                .ThenBy(x => x.Item1.Corp)
                .ThenBy(x => x.Item1.Name)
                .Select(x => InitEquipmentInLibraryButton(x.Item1, totalCount[x.Item1.Name], availableCount[x.Item1.Name], x.Item2))
                .ToList(),
            x => {},
            false);
    }
    
    private Action<GameObject> InitEquipmentInLibraryButton(Equipment equipment, int totalCount, int availableCount, bool canAdd)
    {
        void Init(GameObject gameObj)
        {
            var button = gameObj.GetComponent<EquipmentInLibraryButton>();
            button.Init(equipment, totalCount, availableCount, canAdd);
        }
        return Init;
    }
}