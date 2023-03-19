
public class BlindedEffect : StatusVisualEffect
{
    protected override bool IsActive(Member m) => m.IsBlinded();
}
