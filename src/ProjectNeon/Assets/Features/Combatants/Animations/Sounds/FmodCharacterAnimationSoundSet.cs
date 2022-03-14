using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/FmodCharacterAnimationSoundSet")]
public class FmodCharacterAnimationSoundSet : CharacterAnimationSoundSet
{
    [SerializeField, FMODUnity.EventRef] private string stepSound;
    [SerializeField, FMODUnity.EventRef] private string stepBackSound;
    [SerializeField, FMODUnity.EventRef] private string shootSound;
    [SerializeField, FMODUnity.EventRef] private string rapidShotSound;
    [SerializeField, FMODUnity.EventRef] private string meleeAttack;
    [SerializeField, FMODUnity.EventRef] private string slashAttack;
    [SerializeField, FMODUnity.EventRef] private string stabAttack;
    [SerializeField, FMODUnity.EventRef] private string throwGrenade;
    
    public override void Play(Transform uiSource, CharacterAnimationSoundType sound)
    {
        if (sound == CharacterAnimationSoundType.Step)
            Play(stepSound, uiSource.position);
        if (sound == CharacterAnimationSoundType.StepBack)
            Play(stepBackSound, uiSource.position);
        if (sound == CharacterAnimationSoundType.Shoot)
            Play(shootSound, uiSource.position);
        if (sound == CharacterAnimationSoundType.RapidShot)
            Play(rapidShotSound, uiSource.position);
        if (sound == CharacterAnimationSoundType.MeleeAttack)
            Play(meleeAttack, uiSource.position);
        if (sound == CharacterAnimationSoundType.Slash)
            Play(OrFallback(slashAttack, meleeAttack), uiSource.position);
        if (sound == CharacterAnimationSoundType.Stab)
            Play(OrFallback(stabAttack, meleeAttack), uiSource.position);
        if (sound == CharacterAnimationSoundType.ThrowGrenade)
            Play(throwGrenade, uiSource.position);
    }

    private string OrFallback(string val, string fallback) => string.IsNullOrWhiteSpace(val) ? fallback : val;

    private void Play(string sound, Vector3 pos)
    {
        if (string.IsNullOrWhiteSpace(sound))
            return;
        
        FMODUnity.RuntimeManager.PlayOneShot(sound, pos);
    }
}
