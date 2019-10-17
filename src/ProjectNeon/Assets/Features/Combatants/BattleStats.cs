
/**
 * Stats decorator that lasts entire battle
 */
public abstract class BattleStats : IStats
{
    private IStats origin;

    public virtual int MaxHP => origin.MaxHP;
    public virtual int MaxShield => origin.MaxShield;
    public virtual int Attack => origin.Attack;
    public virtual int Magic => origin.Magic;
    public virtual float Armor => origin.Armor;
    public virtual float Resistance => origin.Resistance;
    public virtual IResourceType[] ResourceTypes => origin.ResourceTypes;
    public IStats Stats => origin;
    
    // @todo #1:15min Kill method Initialization
    public void Init(IStats origin)
    {
        this.origin = origin;
    }
}
