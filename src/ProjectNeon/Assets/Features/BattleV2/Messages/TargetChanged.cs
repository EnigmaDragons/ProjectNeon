
public class TargetChanged
{
    public Maybe<Target> Target { get; }

    public TargetChanged() => Target = Maybe<Target>.Missing();
    public TargetChanged(Target t) => Target = new Maybe<Target>(t);
}
