using FMODUnity;
using UnityEngine;

public class PlayCustomFmodMusicOnStart : MonoBehaviour
{
    [SerializeField, EventRef] private string music;

    private void Start()
    {
        Message.Publish(new PlayCustomFmodMusic(music));
    }
}
