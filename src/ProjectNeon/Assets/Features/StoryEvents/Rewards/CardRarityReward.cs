using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Rewards/Card")]
public class CardRarityReward : StoryResult
{
    [SerializeField] private Rarity rarity;
    [SerializeField] private ShopCardPool cardPool;
    
    public override void Apply(StoryEventContext ctx) 
        => ctx.Party.Add(new ShopSelectionPicker().PickCardsOfRarity(ctx.Party, cardPool, rarity, 1));
}
