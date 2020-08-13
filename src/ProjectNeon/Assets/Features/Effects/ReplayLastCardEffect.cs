
public sealed class ReplayLastCardEffect : Effect
{
    public void Apply(Member source, Target target)
    {
        Message.Publish(new ReplayLastCard());
    }
}
