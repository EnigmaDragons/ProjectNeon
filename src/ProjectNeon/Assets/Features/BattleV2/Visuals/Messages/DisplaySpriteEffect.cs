
public class DisplaySpriteEffect
{
    public SpriteEffectType EffectType { get; }
    
    public Member Target { get; }

    public DisplaySpriteEffect(SpriteEffectType effectType, Member target)
    {
        EffectType = effectType;
        Target = target;
    }
}
