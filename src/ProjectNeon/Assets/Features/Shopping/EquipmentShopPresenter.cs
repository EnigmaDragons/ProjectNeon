using UnityEngine;

public class EquipmentShopPresenter : OnMessage<RefreshShop>
{
    [SerializeField] private EquipmentPool equipment;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private ShopEquipmentPurchaseSlot equipmentPurchasePrototype;
    [SerializeField] private GameObject equipmentParent;
    [SerializeField] private AdventureProgress2 adventure;
    [SerializeField] private ShopState shop;
    [SerializeField] private AllCorps allCorps;
    [SerializeField] private CorpUiBase[] corpBranding;
    [SerializeField] private CorpAffinityUiBase[] affinityUi;

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
    protected override void Execute(RefreshShop msg) => GetMoreInventory();

    public void GetMoreInventory()
    {
        Clear();
        
        var corpAffinity = party.GetCorpAffinity(allCorps.GetMap());
        corpBranding.ForEach(c => c.Init(shop.Corp));
        affinityUi.ForEach(a => a.Init(shop.Corp, corpAffinity));
            
        var isPolyCorp = shop.Corp == allCorps.PolyCorp;
        var priceFactor = isPolyCorp
            ? 1f
            : AffinityPricingAdjustment.PriceFactor(corpAffinity, shop.Corp.Name);
        priceFactor = priceFactor * party.GetCostFactorForEquipment(shop.Corp.Name);
        
        _selection = adventure.CreateLootPicker(party)
            .GenerateEquipmentSelection(equipment, _numEquips, isPolyCorp ? "" : shop.Corp.Name);
        
        _selection.Equipment.ForEach(c => 
            Instantiate(equipmentPurchasePrototype, equipmentParent.transform)
                .Initialized(c, priceFactor));
    }
}

