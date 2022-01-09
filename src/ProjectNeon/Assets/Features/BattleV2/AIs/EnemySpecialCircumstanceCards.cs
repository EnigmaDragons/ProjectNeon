
public class EnemySpecialCircumstanceCards
{
    public CardTypeData DisabledCard { get; }
    public CardTypeData AntiStealthCard { get; }
    public CardTypeData AiGlitchedCard { get; }
    
    public EnemySpecialCircumstanceCards(CardTypeData disabledCard, CardTypeData antiStealthCard, CardTypeData aiGlitchedCard)
    {
        DisabledCard = disabledCard;
        AntiStealthCard = antiStealthCard;
        AiGlitchedCard = aiGlitchedCard;
    }
}
