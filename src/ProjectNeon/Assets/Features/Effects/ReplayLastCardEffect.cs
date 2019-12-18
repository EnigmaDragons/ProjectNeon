


public sealed class ReplayLastCardEffect : Effect
{
    public void Apply(Member source, Target target)
    {
        BattleEvent.Publish(new ReplayLastCard());
    }
}

