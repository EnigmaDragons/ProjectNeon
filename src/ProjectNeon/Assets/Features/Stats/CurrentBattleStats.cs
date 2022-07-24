
public class CurrentBattleStats
{
    public int CardsPlayed;
    public int DamageDealt;
    public int DamageReceived;
    public int HpDamageReceived;
    public int HealingReceived;

    public int HighestPreTurn4CardDamage;

    public void Clear()
    {
        CardsPlayed = 0;
        DamageDealt = 0;
        DamageReceived = 0;
        HpDamageReceived = 0;
        HealingReceived = 0;
        HighestPreTurn4CardDamage = 0;
    }
}
