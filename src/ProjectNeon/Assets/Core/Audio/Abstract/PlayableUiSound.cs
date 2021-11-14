
using UnityEngine;

public abstract class PlayableUiSound : ScriptableObject
{
    public abstract void Play();
    public abstract void Play(Vector3 position);
}
