using UnityEngine;

public class PlayRapidShotOnTransition : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterAnimationSoundPublisher.PlayRapidShot();
    }
}
