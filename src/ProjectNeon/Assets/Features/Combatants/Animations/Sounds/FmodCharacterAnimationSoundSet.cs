using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/FmodCharacterAnimationSoundSet")]
public class FmodCharacterAnimationSoundSet : CharacterAnimationSoundSet
{
    [SerializeField, FMODUnity.EventRef] private string stepSound;
    [SerializeField, FMODUnity.EventRef] private string shootSound;
    [SerializeField, FMODUnity.EventRef] private string meleeAttack;
    
    public override void Play(Member source, Transform uiSource, CharacterAnimationSoundType sound)
    {
        if (sound == CharacterAnimationSoundType.Step)
            Play(stepSound, uiSource.position);
        if (sound == CharacterAnimationSoundType.Shoot)
            Play(shootSound, uiSource.position);
        if (sound == CharacterAnimationSoundType.MeleeAttack)
            Play(meleeAttack, uiSource.position);
    }

    private void Play(string sound, Vector3 pos)
    {
        FMODUnity.RuntimeManager.PlayOneShot(sound, pos);
    }
}
