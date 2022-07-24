using FMOD.Studio;
using UnityEngine;

public class DraftModeSoundGuy : MonoBehaviour
{
    [SerializeField, FMODUnity.EventRef] private string OnItemSelected;
    [SerializeField, FMODUnity.EventRef] private string OnItemHovered;

    private bool _debuggingLoggingEnabled = false;
    private EventInstance _levelUpStinger;

    private void OnEnable()
    {
        Message.Subscribe<CardHovered>(e => PlayOneShot(OnItemHovered, e.UiSource), this);
        Message.Subscribe<ItemHovered>(e => PlayOneShot(OnItemHovered, e.UiSource), this);
        Message.Subscribe<DraftItemPicked>(e => PlayOneShot(OnItemSelected, e.UiSource), this);
    }
    
    private void OnDisable()
    {
        Message.Unsubscribe(this);
    }
    
    private void PlayOneShot(string eventName, Transform uiSource)
    {
        DebugLog($"Draft SFX - {eventName}");
        FMODUnity.RuntimeManager.PlayOneShot(eventName, uiSource.position);
    }

    private void DebugLog(string msg)
    {
        if (_debuggingLoggingEnabled)
            Log.Info(msg);
    }
}
