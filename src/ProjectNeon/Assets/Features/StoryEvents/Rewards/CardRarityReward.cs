using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Rewards/Card")]
public class CardRarityReward : StoryResult
{
    [SerializeField] private Rarity rarity;
    [SerializeField] private ShopCardPool cardPool;

    public override int EstimatedCreditsValue => rarity.CardShopPrice();
    public override bool IsReward => true;
    
    public override void Apply(StoryEventContext ctx)
    {
        var card = new LootPicker(ctx.CurrentStage, ctx.RarityFactors, ctx.Party).PickCards(cardPool, 1, rarity);
        ctx.Party.Add(card);
        Message.Publish(new ShowCardReward(card[0]));
    }

    public override void Preview()
    {
        Message.Publish(new ShowTextResultPreview { Text = $"Gain a random {rarity.ToString()} card", IsReward = true });
    }
}
