using UnityEngine;

public class CharacterSoundsAnimationBinding : MonoBehaviour
{
    public void PlayStepForward() => CharacterAnimationSoundPublisher.PlayStepForward();
    public void PlayStepBack() => CharacterAnimationSoundPublisher.PlayStepBack();
    public void PlayShoot() => CharacterAnimationSoundPublisher.PlayShoot();
    public void PlayRapidShot() => CharacterAnimationSoundPublisher.PlayRapidShot();
}
