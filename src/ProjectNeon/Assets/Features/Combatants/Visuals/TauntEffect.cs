
public class TauntEffect : StatusVisualEffect
{
    protected override bool IsActive(Member m) => m.HasTaunt();
}
