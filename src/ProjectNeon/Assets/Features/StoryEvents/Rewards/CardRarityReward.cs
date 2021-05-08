using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Rewards/Card")]
public class CardRarityReward : StoryResult
{
    [SerializeField] private Rarity rarity;
    [SerializeField] private ShopCardPool cardPool;

    public override int EstimatedCreditsValue => rarity.CardShopPrice();

    public override void Apply(StoryEventContext ctx)
    {
        var card = new ShopSelectionPicker(ctx.RarityFactors, ctx.Party).PickCards(cardPool, 1, rarity);
        ctx.Party.Add(card);
        Message.Publish(new ShowCardReward(card[0]));
    }
}
