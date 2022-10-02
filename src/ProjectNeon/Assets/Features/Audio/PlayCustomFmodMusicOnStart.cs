using FMODUnity;
using UnityEngine;

public class PlayCustomFmodMusicOnStart : MonoBehaviour
{
    [SerializeField, EventRef] private string music;
    [SerializeField] private float delay = 0f;

    private void Start()
    {
        if (delay > 0f)
            this.ExecuteAfterDelay(delay, Play);
        else
            Play();
    }

    private void Play() => Message.Publish(new PlayCustomFmodMusic(music));
}
