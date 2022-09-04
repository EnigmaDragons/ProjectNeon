using System.Linq;

public class NewsCutscenePresenter : BaseCutscenePresenter
{
    public void StartNewscast()
    {
        Reset();
        cutscene.Reset();
        Characters.Clear();
        Characters.Add(narrator);
        
        DebugLog($"Characters in cutscene: {string.Join(", ", Characters.Select(c => c.PrimaryName))}");
        
        DebugLog($"Num Cutscene Segments {cutscene.Current.Segments.Length}");
        MessageGroup.Start(
            new MultiplePayloads("Cutscene Script", cutscene.Current.Segments.Select(s => new ShowCutsceneSegment(s)).Cast<object>().ToArray()), 
            () => Message.Publish(new CutsceneFinished()));
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
        Message.Publish(new HideNewscast());
        MessageGroup.TerminateAndClear();
    }

    protected override void Execute(WinBattleWithRewards msg) { }
}
