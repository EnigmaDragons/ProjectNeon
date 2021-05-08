using UnityEngine;

public class EquipmentShopPresenter : OnMessage<GetFreshEquipmentSet>
{
    [SerializeField] private EquipmentPool equipment;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private ShopEquipmentPurchaseSlot equipmentPurchasePrototype;
    [SerializeField] private GameObject equipmentParent;
    [SerializeField] private AdventureProgress2 adventure;
 
    private ShopSelection _selection;
    private int _numEquips;

    private void Awake()
    {
        _numEquips = equipmentParent.transform.childCount;
        Clear();
    }

    private void Clear()
    {
        if (equipmentParent != null)
            foreach (Transform c in equipmentParent.transform)
                Destroy(c.gameObject);
    }

    protected override void AfterEnable() => GetMoreInventory();
    protected override void Execute(GetFreshEquipmentSet msg) => GetMoreInventory();

    public void GetMoreInventory()
    {
        Clear();
        _selection = new ShopSelectionPicker(adventure.Stage, adventure.CurrentStage.RewardRarityFactors, party)
            .GenerateEquipmentSelection(equipment, _numEquips);
        _selection.Equipment.ForEach(c => 
            Instantiate(equipmentPurchasePrototype, equipmentParent.transform)
                .Initialized(c));
    }
}

