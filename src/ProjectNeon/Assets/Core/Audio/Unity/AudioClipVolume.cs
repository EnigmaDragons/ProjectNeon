
using System;
using UnityEngine;

[Serializable]
public class AudioClipVolume
{
    [SerializeField] private string name;
    public AudioClip clip;
    public float volume = 1f;

    public string Name => string.IsNullOrWhiteSpace(name) ? clip.name : name;
}
