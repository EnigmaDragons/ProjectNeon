using UnityEngine;

public class CharacterSoundsAnimationBinding : MonoBehaviour
{
    public void PlayStepForward() => CharacterAnimationSoundPublisher.PlayStepForward();
    public void PlayStepBack() => CharacterAnimationSoundPublisher.PlayStepBack();
    public void PlayShoot() => CharacterAnimationSoundPublisher.PlayShoot();
    public void PlayRapidShot() => CharacterAnimationSoundPublisher.PlayRapidShot();
    public void PlayMelee() => CharacterAnimationSoundPublisher.PlayMelee();
    public void PlaySlash() => CharacterAnimationSoundPublisher.PlaySlash();
    public void PlayStab() => CharacterAnimationSoundPublisher.PlayStab();
    public void PlayThrowGrenade() => CharacterAnimationSoundPublisher.PlayThrowGrenade();
}
