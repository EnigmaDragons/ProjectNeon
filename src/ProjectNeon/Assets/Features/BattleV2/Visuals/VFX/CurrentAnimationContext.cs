using UnityEngine;

[CreateAssetMenu(menuName = "Only Once/Current Animation")]
public class CurrentAnimationContext : ScriptableObject
{
    public int MemberId;
    public AnimationData AnimationData;
    public Target Target;

    public void SetAnimation(CharacterAnimationRequested animation)
    {
        MemberId = animation.MemberId;
        AnimationData = animation.Animation;
        Target = animation.Target;
    }
}