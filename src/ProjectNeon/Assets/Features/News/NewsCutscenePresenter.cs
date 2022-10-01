using System.Linq;
using DG.Tweening;
using UnityEngine;

public class NewsCutscenePresenter : BaseCutscenePresenter
{
    [SerializeField] private GameObject videoScreen;
    
    public void StartNewscast()
    {
        Reset();
        cutscene.Reset();
        Characters.Clear();
        Characters.Add(narrator);
        
        videoScreen.transform.DOKill();
        videoScreen.transform.rotation = Quaternion.Euler(0, 90, 0);
        videoScreen.transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), 0.6f);

        DebugLog($"Characters in cutscene: {string.Join(", ", Characters.Select(c => c.PrimaryName))}");
        
        DebugLog($"Num Cutscene Segments {cutscene.Current.Segments.Length}");
        MessageGroup.TerminateAndClear();
        this.ExecuteAfterDelay(2, () => 
            MessageGroup.Start(
                new MultiplePayloads("Cutscene Script", cutscene.Current.Segments.Select(s => new ShowCutsceneSegment(s)).Cast<object>().ToArray()), 
                () => Message.Publish(new CutsceneFinished())));
    }
    
    protected override void Execute(SkipCutsceneRequested msg) => FinishCutscene();

    protected override void FinishCutscene()
    {
        if (_finishTriggered)
            return;
        
        _finishTriggered = true;
        DebugLog("News Cutscene Finished");
        narrator.SpeechBubble.ForceHide();
        you.SpeechBubble.ForceHide();
        player.SpeechBubble.ForceHide();
        MessageGroup.TerminateAndClear();
        
        Message.Publish(new OnNewsDismissed(videoScreen.transform));
        videoScreen.transform.DOKill();
        videoScreen.transform.rotation = Quaternion.Euler(0, 0, 0);
        videoScreen.transform.DORotateQuaternion(Quaternion.Euler(0, 90, 0), 0.6f);

        this.ExecuteAfterDelay(0.6f, () => Message.Publish(new HideNewscast()));
    }

    protected override void Execute(WinBattleWithRewards msg) { }
}
