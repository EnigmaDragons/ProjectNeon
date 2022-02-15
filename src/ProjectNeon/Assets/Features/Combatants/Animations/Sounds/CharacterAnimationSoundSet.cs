using UnityEngine;

public abstract class CharacterAnimationSoundSet : ScriptableObject
{
    public abstract void Play(Transform uiSource, CharacterAnimationSoundType sound);
}
