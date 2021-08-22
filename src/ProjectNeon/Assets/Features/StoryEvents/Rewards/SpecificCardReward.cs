using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Rewards/Specific Card")]
public class SpecificCardReward : StoryResult
{
    [SerializeField] private CardType card;

    public override int EstimatedCreditsValue => card.Rarity.CardShopPrice();
    
    public override void Apply(StoryEventContext ctx)
    {
        ctx.Party.Add(card);
        Message.Publish(new ShowCardReward(card));
    }

    public override void Preview()
    {
        Message.Publish(new ShowCardResultPreview { Card = card });
    }
}