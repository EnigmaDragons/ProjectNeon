using UnityEngine;

public class EquipmentShopPresenter : OnMessage<GetFreshEquipmentSet>
{
    [SerializeField] private EquipmentPool equipment;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private ShopEquipmentPurchaseSlot equipmentPurchasePrototype;
    [SerializeField] private GameObject equipmentParent;
    [SerializeField] private AdventureProgress2 adventure;
    [SerializeField] private ShopState shop;
    [SerializeField] private AllCorps allCorps;
    [SerializeField] private CorpUiBase[] corpBranding;

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
        corpBranding.ForEach(c => c.Init(shop.Corp));
        Clear();
        var isPolyCorp = shop.Corp == allCorps.PolyCorp;
        var priceFactor = isPolyCorp
            ? 1f
            : AffinityPricingAdjustment.PriceFactor(party.GetCorpAffinity(allCorps.GetMap()), shop.Corp.Name);
        DevLog.Info($"{shop.Corp.Name} - Price Factor {priceFactor}");
        
        _selection = new ShopSelectionPicker(adventure.CurrentChapterNumber, adventure.CurrentChapter.RewardRarityFactors, party)
            .GenerateEquipmentSelection(equipment, _numEquips, isPolyCorp ? "" : shop.Corp.Name);
        
        _selection.Equipment.ForEach(c => 
            Instantiate(equipmentPurchasePrototype, equipmentParent.transform)
                .Initialized(c, priceFactor));
    }
}

