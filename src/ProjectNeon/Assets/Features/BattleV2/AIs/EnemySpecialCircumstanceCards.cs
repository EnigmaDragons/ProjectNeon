
public class EnemySpecialCircumstanceCards
{
    public CardType DisabledCard { get; }
    public CardType AntiStealthCard { get; }
    public CardType AiGlitchedCard { get; }
    
    public EnemySpecialCircumstanceCards(CardType disabledCard, CardType antiStealthCard, CardType aiGlitchedCard)
    {
        DisabledCard = disabledCard;
        AntiStealthCard = antiStealthCard;
        AiGlitchedCard = aiGlitchedCard;
    }
}
