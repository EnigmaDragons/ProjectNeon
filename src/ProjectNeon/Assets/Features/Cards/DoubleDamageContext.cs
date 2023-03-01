public class DoubleDamageContext
{
    private readonly Member _source;
    private readonly bool _canDoubleDamage;
    private bool _hasUsedDoubleDamage;

    public DoubleDamageContext(Member source, bool canDoubleDamage)
    {
        _source = source;
        _canDoubleDamage = canDoubleDamage;
    }

    public int WithDoubleDamage(int doubleDamage)
    {
        if (!_canDoubleDamage)
            return doubleDamage;
        if (!_hasUsedDoubleDamage)
            _source.State.Adjust(TemporalStatType.DoubleDamage, -1);
        _hasUsedDoubleDamage = true;
        return doubleDamage * 2;
    }
}