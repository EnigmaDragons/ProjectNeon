using UnityEngine;

public abstract class CharacterAnimationSoundSet : ScriptableObject
{
    public abstract void Play(Member source, Transform uiSource, CharacterAnimationSoundType sound);
}
