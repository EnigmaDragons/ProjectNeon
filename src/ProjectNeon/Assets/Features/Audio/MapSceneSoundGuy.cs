using UnityEngine;


public class MapSceneSoundGuy : MonoBehaviour
{
    [SerializeField, FMODUnity.EventRef] private string OnLevelUpClicked;

    private bool debuggingLoggingEnabled = false;

    private void OnEnable()
    {
        Message.Subscribe<LevelUpClicked>(e => PlayOneShot(OnLevelUpClicked, e.UiSource), this);
        
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
    }

    private void PlayOneShot(string eventName, Transform uiSource)
        => FMODUnity.RuntimeManager.PlayOneShot(eventName, uiSource.position);

    private void DebugLog(string msg)
    {
        if (debuggingLoggingEnabled)
            Log.Info(msg);
    }
}
