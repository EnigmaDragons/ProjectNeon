using UnityEngine;

public class MemberSpawned
{
    public Member Member { get; }
    public Transform Transform { get; }

    public MemberSpawned(Member m, Transform t)
    {
        Member = m;
        Transform = t;
    }
}
