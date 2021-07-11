using System;
using System.Linq;
using UnityEngine;

public class FullGearLibraryUI : MonoBehaviour
{
    [SerializeField] private PageViewer pageViewer;
    [SerializeField] private EquipmentInLibraryButton equipmentInLibraryButtonTemplate;
    [SerializeField] private EquipmentPool gearPool;
    [SerializeField] private GameObject emptyObj;
    
    private void Awake() => GenerateLibrary();

    private void GenerateLibrary()
    {
        pageViewer.Init(
            equipmentInLibraryButtonTemplate.gameObject, 
            emptyObj, 
            gearPool.All
                .Where(c => c.Slot != EquipmentSlot.Permanent)
                .Where(c => !c.IsRandomlyGenerated())
                .OrderByDescending(c => c.Corp)
                .ThenByDescending(c => c.Archetypes.Any())
                .ThenBy(c => c.GetArchetypeKey())
                .ThenBy(c => c.Slot)
                .ThenBy(c => c.Rarity)
                .ThenBy(c => c.Name)
                .Select(InitEquipmentInLibraryButton)
                .ToList(), 
            x => {},
            false);
    }
    
    private Action<GameObject> InitEquipmentInLibraryButton(Equipment equipment) 
        => gameObj => gameObj.GetComponent<EquipmentInLibraryButton>().InitInfoOnly(equipment);
}
