using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class StealthTransparency : OnMessage<MemberStateChanged>
{
    private SpriteRenderer _renderer;
    private Maybe<Member> _member = Maybe<Member>.Missing();
    private static readonly Color _stealthTint = new Color(255, 255, 255, 0.3f);
    
    private void Awake() => _renderer = GetComponent<SpriteRenderer>();

    public void Init(Member m)
    {
        _member = m;
        Render(m);
    }

    protected override void Execute(MemberStateChanged msg)
    {
        _member.IfPresent(m =>
        {
            if (msg.State.MemberId == _member.Value.Id)
                Render(m);
        });
    }
    
    private void Render(Member m) 
        => _renderer.color = m.State[TemporalStatType.Stealth] > 0 
            ? _stealthTint 
            : Color.white;
}
