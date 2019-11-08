public sealed class HealFlatForTurnsOnTurnStart : Effect
{
    private int _turns;
    private int _amount;

    public HealFlatForTurnsOnTurnStart(int amount, int turns) {
        _turns = turns;
        _amount = amount;
    }

    public void Apply(Member source, Target target)
    {
        new EffectOnTurnStart(new ForNumberOfTurns(new Heal(_amount), _turns)).Apply(source, target);
    }

}
