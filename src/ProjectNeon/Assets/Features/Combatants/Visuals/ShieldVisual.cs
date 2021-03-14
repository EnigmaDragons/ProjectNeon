using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ShieldVisual : OnMessage<MemberStateChanged>
{
    private float zRot = 0.4f;
    private SpriteRenderer _renderer;
    private Maybe<Member> _member = Maybe<Member>.Missing();
    
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
    {
        Log.Info("Render");
        _renderer.color = new Color(255f, 255f, 255f, m.CurrentShield() / ((float)m.MaxShield() * 3f));
    }

    private void FixedUpdate()
    {
        _member.IfPresent(m =>
        {
            if (m.CurrentShield() > 0)
                transform.Rotate(0, 0, zRot);
        });
    }
}