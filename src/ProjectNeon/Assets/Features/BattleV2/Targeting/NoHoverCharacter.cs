using System;

public class NoHoverCharacter : HoverCharacter
{
    private Maybe<Member> _member = Maybe<Member>.Missing();

    public void Init(Member member) => _member = member;
    public void SetIsHovered() {}

    public void SetAction(Action confirmAction, Action cancelAction) {}
    public void Revert() { }

    public bool IsInitialized => _member.IsPresent;
    public Member Member => _member.Value;
}
