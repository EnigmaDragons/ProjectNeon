using CharacterCreator2D;
using UnityEngine;

public class CharacterCreatorStealthTransparency : OnMessage<MemberStateChanged>
{
    [SerializeField] private CharacterViewer viewer;
    
    private Maybe<Member> _member = Maybe<Member>.Missing();
    private static readonly Color _stealthTint = new Color(1f, 1f, 1f, 0.2f);
    
    public void Init(Member m)
    {
        _member = m;
        Render(m);
    }

    protected override void Execute(MemberStateChanged msg) 
        => _member.IfPresentAndMatches(m => m.Id == msg.State.MemberId, Render);

    private void Render(Member m) 
        => viewer.TintColor = m.State[TemporalStatType.Stealth] > 0 
            ? _stealthTint 
            : Color.white;
}