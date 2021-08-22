using UnityEngine;

public class StoryEventSounds : OnMessage<ShowStoryEventResolution>
{
    [SerializeField] private UiSfxPlayer player;
    [SerializeField] private AudioClipVolume goodOutcome;
    [SerializeField] private AudioClipVolume badOutcome;
    [SerializeField] private FloatReference delay = new FloatReference(1f);
    
    protected override void Execute(ShowStoryEventResolution msg)
    {
        if (msg.IsGoodOutcome)
            this.ExecuteAfterDelay(() => player.Play(goodOutcome), delay);
        else if (msg.IsBadOutcome)
            this.ExecuteAfterDelay(() => player.Play(badOutcome), delay);
    }
}