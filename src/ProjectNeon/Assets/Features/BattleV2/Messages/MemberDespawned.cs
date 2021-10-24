using UnityEngine;

public class MemberDespawned
{
    public Member Member { get; }
    public Vector3 FormerPosition { get; }

    public MemberDespawned(Member m, Vector3 formerPosition)
    {
        Member = m;
        FormerPosition = formerPosition;
    }
}
