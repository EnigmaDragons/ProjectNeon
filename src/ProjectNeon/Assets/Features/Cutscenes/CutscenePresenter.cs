using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CutscenePresenter : OnMessage<AdvanceCutsceneRequested, ShowCharacterDialogueLine, ShowCutsceneSegment, CutsceneFinished>
{
    [SerializeField] private Navigator navigator;
    [SerializeField] private FloatReference cutsceneFinishNavigationDelay = new FloatReference(1f);
    [SerializeField] private FloatReference dialogueWaitDelay = new FloatReference(2f);
    [SerializeField] private CurrentCutscene cutscene;
    [SerializeField] private GameObject settingParent;

    private bool _debugLoggingEnabled = true;

    private readonly List<CutsceneCharacter> _characters = new List<CutsceneCharacter>();
    
    private void Start()
    {
        _characters.Clear();
        cutscene.Current.Setting.SpawnTo(settingParent);
        var characters = settingParent.GetComponentsInChildren<CutsceneCharacter>();
        characters.ForEach(c => _characters.Add(c));
        
        DebugLog($"Characters in cutscene: {string.Join(", ", _characters.Select(c => c.PrimaryName))}");
        
        DebugLog($"Num Cutscene Segments {cutscene.Current.Segments.Length}");
        MessageGroup.Start(
            new MultiplePayloads(cutscene.Current.Segments.Select(s => new ShowCutsceneSegment(s)).Cast<object>().ToArray()), 
            () => Message.Publish(new CutsceneFinished()));
    }
    
    protected override void Execute(AdvanceCutsceneRequested msg)
    {
    }

    protected override void Execute(ShowCharacterDialogueLine msg)
    {
        DebugLog("Show Character Dialogue Line");
        _characters.FirstOrMaybe(c => c.Matches(msg.CharacterAlias))
            .ExecuteIfPresentOrElse(
                c => c.SpeechBubble.Display(msg.Text, true, () => this.ExecuteAfterDelay(FinishCurrentSegment, dialogueWaitDelay)),
                FinishCurrentSegment);
    }

    protected override void Execute(ShowCutsceneSegment msg)
    {
        DebugLog("Show Cutscene Segment");
        _characters.ForEach(c => c.SpeechBubble.ForceHide());
        AllCutsceneSegments.Create(msg.SegmentData).Start();
    }

    protected override void Execute(CutsceneFinished msg)
    {
        DebugLog("Cutscene Finished");
        this.ExecuteAfterDelay(navigator.NavigateToGameSceneV4, cutsceneFinishNavigationDelay);
    }

    private void FinishCurrentSegment()
    {
        DebugLog("Segment Finished");
        Message.Publish(new Finished<ShowCutsceneSegment>());
    }

    private void DebugLog(string msg)
    {
        if (_debugLoggingEnabled)
            Log.Info("Cutscene Presenter - " + msg);
    }
}
