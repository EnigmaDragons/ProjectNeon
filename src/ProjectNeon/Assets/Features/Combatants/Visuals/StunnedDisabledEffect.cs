
public class StunnedDisabledEffect : StatusVisualEffect
{
    protected override bool IsActive(Member m) => m.IsStunnedForCard() || m.IsDisabled();
}
