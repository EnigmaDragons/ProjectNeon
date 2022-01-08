
public class EnemySpecialCircumstanceCards
{
    public CardTypeData DisabledCard { get; }
    public CardTypeData AntiStealthCard { get; }
    
    public EnemySpecialCircumstanceCards(CardTypeData disabledCard, CardTypeData antiStealthCard)
    {
        DisabledCard = disabledCard;
        AntiStealthCard = antiStealthCard;
    }
}