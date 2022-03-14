
public static class CharacterAnimationSoundPublisher
{
    public static void PlayStepForward() => Play(CharacterAnimationSoundType.Step);
    public static void PlayStepBack() => Play(CharacterAnimationSoundType.StepBack);
    public static void PlayShoot() => Play(CharacterAnimationSoundType.Shoot);
    public static void PlayRapidShot() => Play(CharacterAnimationSoundType.RapidShot);
    public static void PlayMelee() => Play(CharacterAnimationSoundType.MeleeAttack);
    public static void PlaySlash() => Play(CharacterAnimationSoundType.Slash);
    public static void PlayStab() => Play(CharacterAnimationSoundType.Stab);
    public static void PlayThrowGrenade() => Play(CharacterAnimationSoundType.ThrowGrenade);
    
    public static void Play(CharacterAnimationSoundType sound)
    {
        Log.Info($"SFX: Play {sound}");
        Message.Publish(new PlayCharacterAnimSound {SoundType = sound});
    }
}
