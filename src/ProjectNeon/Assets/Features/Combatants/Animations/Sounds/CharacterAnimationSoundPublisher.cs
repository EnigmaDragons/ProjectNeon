
public static class CharacterAnimationSoundPublisher
{
    public static void PlayStepForward() => Play(CharacterAnimationSoundType.Step);
    public static void PlayStepBack() => Play(CharacterAnimationSoundType.StepBack);
    public static void PlayShoot() => Play(CharacterAnimationSoundType.Shoot);
    public static void PlayRapidShot() => Play(CharacterAnimationSoundType.RapidShot);
    
    public static void Play(CharacterAnimationSoundType sound)
    {
        Log.Info($"Play {sound}");
        Message.Publish(new PlayCharacterAnimSound {SoundType = sound});
    }
}
