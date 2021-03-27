using System;
using UnityEngine;

public interface HoverCharacter
{
    void Init(Member m);
    void Set(Material material);
    void SetIsHovered();
    void SetAction(Action confirmAction, Action cancelAction);
    void Revert();

    bool IsInitialized { get; }
    Member Member { get; }
}
