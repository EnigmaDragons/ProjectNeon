using UnityEngine;
using UnityEngine.Video;

public class SwapVideosIfLinux : MonoBehaviour
{
    [SerializeField] private BoolVariable isLinux;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip linuxVideoClip;
    
    private void Awake()
    {
        if (isLinux.Value)
            videoPlayer.clip = linuxVideoClip;
        videoPlayer.enabled = true;
    }
}