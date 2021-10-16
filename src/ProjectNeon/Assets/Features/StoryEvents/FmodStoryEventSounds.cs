
using UnityEngine;

public class FmodStoryEventSounds : OnMessage<ShowStoryEventResolution>
{
    [SerializeField, FMODUnity.EventRef] private string goodOutcome;
    [SerializeField, FMODUnity.EventRef] private string badOutcome;
    [SerializeField] private FloatReference delay = new FloatReference(1f);

    protected override void Execute(ShowStoryEventResolution msg)
    {
        if (msg.IsGoodOutcome)
            this.ExecuteAfterDelay(() => PlayOneShot(goodOutcome, transform), delay);
        else if (msg.IsBadOutcome)
            this.ExecuteAfterDelay(() => PlayOneShot(badOutcome, transform), delay);
    }
    
    private void PlayOneShot(string eventName, Transform uiSource)
        => FMODUnity.RuntimeManager.PlayOneShot(eventName, uiSource.position);
}