using System;
using System.Linq;
using UnityEngine;

public class FullGearLibraryUI : MonoBehaviour
{
    [SerializeField] private PageViewer pageViewer;
    [SerializeField] private EquipmentInLibraryButton equipmentInLibraryButtonTemplate;
    [SerializeField] private EquipmentPool gearPool;
    [SerializeField] private GameObject emptyObj;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private bool cheatGainEquipment;
    
    private void Awake() => GenerateLibrary();

    private void GenerateLibrary()
    {
        pageViewer.Init(
            equipmentInLibraryButtonTemplate.gameObject, 
            emptyObj, 
            gearPool.All
                .Where(c => c.Slot == EquipmentSlot.Augmentation)
                .OrderByDescending(c => c.Archetypes.Any())
                .ThenBy(c => c.GetArchetypeKey())
                .ThenBy(c => c.Rarity)
                .ThenBy(c => c.Corp)
                .ThenBy(c => c.Slot)
                .ThenBy(c => c.Name)
                .Select(InitEquipmentInLibraryButton)
                .ToList(), 
            x => {},
            false);
    }
    
    private Action<GameObject> InitEquipmentInLibraryButton(StaticEquipment equipment) 
        => gameObj => gameObj.GetComponent<EquipmentInLibraryButton>().InitInfoOnly(equipment, cheatGainEquipment ? () =>
            {
                party.Add(equipment);
                Message.Publish(new ToggleGearLibrary());
            }
            : (Action)(() => {}));
}
