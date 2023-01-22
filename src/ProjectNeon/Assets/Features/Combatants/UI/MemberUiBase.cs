using UnityEngine;

public abstract class MemberUiBase : MonoBehaviour, IMemberUi
{
    public abstract void Init(Member m);
}
