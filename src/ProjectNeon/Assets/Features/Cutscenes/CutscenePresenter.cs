using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CutscenePresenter : MonoBehaviour
{
    [SerializeField] private Navigator navigator;
    [SerializeField] private FloatReference cutsceneFinishNavigationDelay = new FloatReference(1f);
    [SerializeField] private FloatReference dialogueWaitDelay = new FloatReference(2f);
    [SerializeField] private CurrentCutscene cutscene;
    [SerializeField] private GameObject settingParent;
    [SerializeField] private CutsceneCharacter narrator;

    private CutsceneSegment _currentSegment;
    private bool _debugLoggingEnabled = true;
    private bool _finishTriggered = false;

    private readonly List<CutsceneCharacter> _characters = new List<CutsceneCharacter>();

    private void OnEnable()
    {
        narrator.Init(new [] { CutsceneCharacterAliases.Narrator });
        Message.Subscribe<ShowCutsceneSegment>(Execute, this);
        Message.Subscribe<ShowCharacterDialogueLine>(Execute, this);
        Message.Subscribe<FullyDisplayDialogueLine>(Execute, this);
        Message.Subscribe<CutsceneFinished>(Execute, this);
        Message.Subscribe<SkipCutsceneRequested>(Execute, this);
        Message.Subscribe<AdvanceCutsceneRequested>(Execute, this);
    }

    private void Execute(FullyDisplayDialogueLine msg)
    {        
        DebugLog("Fully Display Character Dialogue Line");
        _characters.FirstOrMaybe(c => c.Matches(msg.CharacterAlias))
            .IfPresent(c => c.SpeechBubble.Proceed());
    }

    private void Execute(AdvanceCutsceneRequested msg)
    {
        if (_currentSegment != null)
        {
            DebugLog("Advance Cutscene");
            _currentSegment.FastForwardToFinishInstantly();
        }
    }

    private void Execute(SkipCutsceneRequested msg)
    {
        DebugLog("Cutscene Skipped");
        navigator.NavigateToGameSceneV4();
    }

    private void Start()
    {
        _characters.Clear();
        _characters.Add(narrator);
        
        cutscene.Current.Setting.SpawnTo(settingParent);
        var characters = settingParent.GetComponentsInChildren<CutsceneCharacter>();
        characters.ForEach(c => _characters.Add(c));
        
        DebugLog($"Characters in cutscene: {string.Join(", ", _characters.Select(c => c.PrimaryName))}");
        
        DebugLog($"Num Cutscene Segments {cutscene.Current.Segments.Length}");
        MessageGroup.Start(
            new MultiplePayloads(cutscene.Current.Segments.Select(s => new ShowCutsceneSegment(s)).Cast<object>().ToArray()), 
            () => Message.Publish(new CutsceneFinished()));
    }

    private void Execute(ShowCharacterDialogueLine msg)
    {
        DebugLog($"Show Character Dialogue Line {msg.CharacterAlias}");
        _characters.FirstOrMaybe(c => c.Matches(msg.CharacterAlias))
            .ExecuteIfPresentOrElse(
                c => c.SpeechBubble.Display(msg.Text, shouldAutoProceed: true, manualInterventionDisablesAuto: false, 
                    () => this.ExecuteAfterDelay(FinishCurrentSegment, dialogueWaitDelay)),
                FinishCurrentSegment);
    }

    private void Execute(ShowCutsceneSegment msg)
    {
        DebugLog("Show Cutscene Segment");
        _characters.ForEach(c => c.SpeechBubble.ForceHide());
        _currentSegment = AllCutsceneSegments.Create(msg.SegmentData);
        _currentSegment.Start();
    }

    private void Execute(CutsceneFinished msg)
    {
        if (_finishTriggered)
            return;
        
        DebugLog("Cutscene Finished");
        _finishTriggered = true;
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
