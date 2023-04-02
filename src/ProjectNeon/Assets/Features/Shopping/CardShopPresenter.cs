using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardShopPresenter : OnMessage<RefreshShop, CardPurchased>
{
    [SerializeField] private ShopCardPool cards;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private ShopCardPurchaseSlot cardPurchasePrototype;
    [SerializeField] private GameObject cardParent;
    [SerializeField] private CurrentAdventureProgress adventureProgress;
    [SerializeField] private DeterminedNodeInfo nodeInfo;
    [SerializeField] private CurrentMapSegmentV5 map;

    private int _numCards;
    private ShopSelection _selection;
    private List<CardTypeData> _purchases = new List<CardTypeData>();

    public ShopSelection Selection => _selection;
    
    private void Awake()
    {
        _numCards = cardParent.transform.childCount;
        Clear();
    }

    private void Clear()
    {
        _selection = null;
        if (cardParent != null)
            foreach (Transform c in cardParent.transform)
                Destroy(c.gameObject);
        _purchases = new List<CardTypeData>();
    }

    protected override void Execute(RefreshShop msg) => GetMoreInventory();
    protected override void Execute(CardPurchased msg) => _purchases.Add(msg.Card);

    protected override void AfterEnable() => GetMoreInventory();
    protected override void AfterDisable()
    {
        PublishShopPurchaseMetricIfRelevant();
        if (_selection.Cards.Count == _numCards)
            Achievements.Record(Achievement.MiscShoppingSpree);
        _selection = null;
        map.DisableSavingCurrentNode();
        nodeInfo.CardShopSelection = Maybe<CardType[]>.Missing();
        Message.Publish(new AutoSaveRequested());
    }

    public void GetMoreInventory()
    {
        PublishShopPurchaseMetricIfRelevant();
        Clear();
        if (nodeInfo.CardShopSelection.IsMissing)
        {
            var selection = adventureProgress.AdventureProgress
                .CreateLootPicker(party)
                .GenerateCardSelection(cards, _numCards);
            nodeInfo.CardShopSelection = selection.Cards.Cast<CardType>().ToArray();
            Message.Publish(new SaveDeterminationsRequested());
        }
        _selection = new ShopSelection(new List<StaticEquipment>(), new List<CardTypeData>(nodeInfo.CardShopSelection.Value));
        var cardsWithOwners = _selection.Cards.Select(c => c.ToNonBattleCard(party));
        cardsWithOwners.ForEach(c => 
            Instantiate(cardPurchasePrototype, cardParent.transform)
                .Initialized(c));
    }

    private void PublishShopPurchaseMetricIfRelevant()
    {
        if (_selection != null)
            AllMetrics.PublishCardShopPurchases(_selection.Cards.Select(c => c.Name).ToArray(),
                _purchases.Select(p => p.Name).ToArray());
    }
}
