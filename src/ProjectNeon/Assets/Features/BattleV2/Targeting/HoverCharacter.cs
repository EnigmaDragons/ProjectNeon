using System;

public interface HoverCharacter
{
    void Init(Member member);
    void SetIsHovered();
    void SetAction(Action confirmAction, Action cancelAction);
    void Revert();

    bool IsInitialized { get; }
    Member Member { get; }
}
