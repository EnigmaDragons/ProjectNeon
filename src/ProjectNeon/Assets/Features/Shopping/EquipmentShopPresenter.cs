using UnityEngine;

public class EquipmentShopPresenter : OnMessage<GetFreshEquipmentSet>
{
    [SerializeField] private EquipmentPool equipment;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private ShopEquipmentPurchaseSlot equipmentPurchasePrototype;
    [SerializeField] private GameObject equipmentParent;
    [SerializeField] private AdventureProgress2 adventure;
    [SerializeField] private ShopState shop;
    [SerializeField] private AllCorps corps;

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
    protected override void AfterDisable() => Message.Publish(new AutoSaveRequested());
    protected override void Execute(GetFreshEquipmentSet msg) => GetMoreInventory();

    public void GetMoreInventory()
    {
        Clear();
        _selection = new ShopSelectionPicker(adventure.CurrentChapterNumber, adventure.CurrentChapter.RewardRarityFactors, party)
            .GenerateEquipmentSelection(equipment, _numEquips, shop.Corp == corps.PolyCorp.Name ? "" : shop.Corp);
        _selection.Equipment.ForEach(c => 
            Instantiate(equipmentPurchasePrototype, equipmentParent.transform)
                .Initialized(c));
    }
}

