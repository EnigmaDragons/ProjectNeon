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
    
    private HeroCharacter _selectedHero;
    
    protected override void Execute(EquipmentPickerCurrentGearChanged msg) => GenerateLibrary();
    protected override void Execute(DeckBuilderFiltersChanged msg) => GenerateLibrary();
    
    public void GenerateLibrary()
    {
        _selectedHero = builderState.SelectedHeroesDeck.Hero.Character;
        var equipUsage = new List<(Equipment, bool)>();
        partyState.Equipment.Available
            .Where(e => e.Archetypes.All(_selectedHero.Archetypes.Contains)
                && (builderState.ShowRarities.None() 
                    || builderState.ShowRarities.Contains(e.Rarity))
                && (builderState.ShowEquipmentSlots.None()
                    || builderState.ShowEquipmentSlots.Contains(e.Slot)))
            .ForEach(x => equipUsage.Add((x, x.Slot != EquipmentSlot.Augmentation || builderState.SelectedHeroesDeck.Hero.Equipment.Augments.Length != 3)));
        partyState.Equipment.Equipped
            .Where(e => e.Archetypes.All(_selectedHero.Archetypes.Contains)
                && (builderState.ShowRarities.None() 
                    || builderState.ShowRarities.Contains(e.Rarity))
                && (builderState.ShowEquipmentSlots.None()
                    || builderState.ShowEquipmentSlots.Contains(e.Slot)))
            .ForEach(x => equipUsage.Add((x, false)));
        noEquipmentMessage.SetActive(!partyState.Equipment.All.Any());
        pageViewer.Init(
            equipmentInLibraryButtonTemplate.gameObject,
            emptyEquipment,
            equipUsage
                .OrderBy(x => x.Item1.Rarity)
                .ThenBy(x => x.Item1.Name)
                .Select(x => InitEquipmentInLibraryButton(x.Item1, x.Item2))
                .ToList(),
            x => {},
            false);
    }
    
    private Action<GameObject> InitEquipmentInLibraryButton(Equipment equipment, bool canAdd)
    {
        void Init(GameObject gameObj)
        {
            var button = gameObj.GetComponent<EquipmentInLibraryButton>();
            button.Init(equipment, canAdd);
        }
        return Init;
    }
}