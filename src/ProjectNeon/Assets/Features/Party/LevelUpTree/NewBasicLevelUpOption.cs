using UnityEngine;

[CreateAssetMenu(menuName = "Hero/LevelUps/BasicCard")]
public class NewBasicLevelUpOption : HeroLevelUpOption
{
    [SerializeField] private CardType card;
    
    public override string IconName => "";
    public override string Description => $"New Basic:\\n{card.Name}";
    
    public override void Apply(Hero h) => h.SetBasic(card);

    public override void ShowDetail() => Message.Publish(new ShowDetailedCardView(card));
    public override bool HasDetail => true;
}