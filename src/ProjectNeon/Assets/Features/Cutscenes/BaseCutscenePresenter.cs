using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseCutscenePresenter : MonoBehaviour
{
    [SerializeField] protected CurrentAdventureProgress progress;
    [SerializeField] protected FloatReference dialogueWaitDelay = new FloatReference(2f);
    [SerializeField] protected CurrentCutscene cutscene;
    [SerializeField] protected CutsceneCharacter narrator;
    [SerializeField] protected CutsceneCharacter you;
    [SerializeField] protected CutsceneCharacter player;
    [SerializeField] protected Image fadeDarken;
    
    protected CutsceneSegment _currentSegment;
    protected bool _debugLoggingEnabled = true;
    protected bool _finishTriggered = false;
    protected bool _waitFinishTriggered = false;
    private bool _skippable = true;
    
    protected readonly List<CutsceneCharacter> Characters = new List<CutsceneCharacter>();
    protected readonly Dictionary<string, List<GameObject>> CharacterAdditionalVisuals = new Dictionary<string, List<GameObject>>();
    
    private void OnEnable()
    {
        fadeDarken.color = fadeDarken.color.WithAlpha(0f);
        narrator.Init(new [] { CutsceneCharacterAliases.Narrator });
        you.Init(new [] { CutsceneCharacterAliases.You });
        player.Init(new [] { CutsceneCharacterAliases.Player });
        Message.Subscribe<ShowCutsceneSegment>(Execute, this);
        Message.Subscribe<ShowCharacterDialogueLine>(Execute, this);
        Message.Subscribe<FullyDisplayDialogueLine>(Execute, this);
        Message.Subscribe<CutsceneFinished>(Execute, this);
        Message.Subscribe<SkipCutsceneRequested>(Execute, this);
        Message.Subscribe<AdvanceCutsceneRequested>(Execute, this);
        Message.Subscribe<CutsceneWaitRequested>(Execute, this);
        Message.Subscribe<FinishCutsceneWaitEarlyRequested>(Execute, this);
        Message.Subscribe<RecordStoryStateRequested>(Execute, this);
        Message.Subscribe<HideCharacterRequested>(Execute, this);
        Message.Subscribe<ShowCharacterRequested>(Execute, this);
        Message.Subscribe<WinBattleWithRewards>(Execute, this);
        Message.Subscribe<CutsceneFadeRequested>(Execute, this);
        Message.Subscribe<TriggerCutsceneEvent>(Execute, this);
    }

    private void Execute(TriggerCutsceneEvent msg) => this.ExecuteAfterDelay(msg.DurationBeforeContinue, FinishCurrentSegment);

    private void Execute(CutsceneFadeRequested msg)
    {
        if (fadeDarken == null)
        {
            Log.Error("Missing Cutscene Darken Image for Camera Fades");
            FinishCurrentSegment();
            return;
        }
        
        var targetAlpha = msg.FadeIn ? 0f : 1f;
        fadeDarken.DOColor(fadeDarken.color.WithAlpha(targetAlpha), msg.Duration);
        this.ExecuteAfterDelay(msg.Duration, FinishCurrentSegment);
    }

    protected void Reset()
    {
        _currentSegment = null;
        _finishTriggered = false;
        _waitFinishTriggered = false;
        _skippable = true;
    }
    
    private void OnDisable()
    {
        Message.Unsubscribe(this);
    }
    
    private void Execute(CutsceneWaitRequested msg)
    {
        if (_finishTriggered)
            return;
        
        _waitFinishTriggered = false;
        this.ExecuteAfterDelay(msg.Duration, () =>
        {
            if (gameObject != null)
                FinishWait();
        });
    }

    private void Execute(RecordStoryStateRequested msg)
    {
        if (_finishTriggered)
            return;

        progress.AdventureProgress.SetStoryState(msg.State, true);
        FinishCurrentSegment();
    }
    
    private void Execute(HideCharacterRequested msg)
    {
        if (_finishTriggered)
            return;

        Characters.FirstOrMaybe(c => c.Matches(msg.CharacterAlias)).ExecuteIfPresentOrElse(x =>
        {
            x.gameObject.SetActive(false);
            if (CharacterAdditionalVisuals.TryGetValue(msg.CharacterAlias, out var items))
                items.ForEach(g => g.SetActive(false));
            FinishCurrentSegment();
        }, FinishCurrentSegment);
    }
    
    private void Execute(ShowCharacterRequested msg)
    {
        if (_finishTriggered)
            return;

        Characters.FirstOrMaybe(c => c.Matches(msg.CharacterAlias)).ExecuteIfPresentOrElse(x =>
        {
            x.gameObject.SetActive(true);
            FinishCurrentSegment();
        }, FinishCurrentSegment);
    }
    
    private void Execute(FinishCutsceneWaitEarlyRequested msg) => FinishWait();

    private void FinishWait()
    {
        if (_waitFinishTriggered)
            return;

        Debug.Log("Finish Wait", this);
        _waitFinishTriggered = true;
        FinishCurrentSegment();
    }
    
    private void Execute(FullyDisplayDialogueLine msg)
    {   
        if (_finishTriggered)
            return;
        
        DebugLog("Fully Display Character Dialogue Line");
        Characters.FirstOrMaybe(c => c.Matches(msg.CharacterAlias))
            .IfPresent(c => c.SpeechBubble.Proceed());
    }

    private void Execute(AdvanceCutsceneRequested msg)
    {        
        if (_finishTriggered)
            return;
        
        if (_currentSegment != null && _skippable)
        {
            DebugLog("Advance Cutscene");
            _currentSegment.FastForwardToFinishInstantly();
        }
    }
    
    private void Execute(ShowCharacterDialogueLine msg)
    {        
        if (_finishTriggered)
            return;

        if (msg.CharacterAlias == null)
        {
            Log.Error($"Cutscene Error {cutscene.Current.name} - No Character Provided for Line: {msg.Text}");
            FinishCurrentSegment();
            return;
        }

        DebugLog($"Show Character Dialogue Line {msg.CharacterAlias}");
        try
        {
            Characters.FirstOrMaybe(c => c.Matches(msg.CharacterAlias))
                .ExecuteIfPresentOrElse(c =>
                    {
                        var useAutoAdvance = CurrentGameOptions.Data.UseAutoAdvance;
                        c.SetTalkingState(true);
                        var speech = c.SpeechBubble;
                        speech.SetAllowManualAdvance(!useAutoAdvance);
                        speech.SetOnFullyShown(() => c.SetTalkingState(false));
                        if (useAutoAdvance)
                            speech.Display(msg.Text, shouldAutoProceed: true, manualInterventionDisablesAuto: false,
                                () => Async.ExecuteAfterDelay(dialogueWaitDelay, FinishCurrentSegment));
                        else
                            speech.Display(msg.Text, shouldAutoProceed: false, FinishCurrentSegment);
                    },
                    () =>
                    {
                        Log.Error($"Cutscene Error {cutscene.Current.name} - Character Not Found in Set: {msg.CharacterAlias}");
                        FinishCurrentSegment();
                    });
        }
        catch (Exception e)
        {
            Log.Warn(e.StackTrace);
            FinishCurrentSegment();
        }
    }

    private void Execute(ShowCutsceneSegment msg)
    {
        if (_finishTriggered)
            return;

        DebugLog(progress.AdventureProgress.GetType().Name);
        foreach (var required in msg.SegmentData.RequiredStates)
        {
            var isTrue = progress.AdventureProgress.IsTrue(required);
            DebugLog($"Required: {required.Value}. IsTrue: {isTrue}");
        }

        if (msg.SegmentData.ShouldSkip(x => progress.AdventureProgress.IsTrue(x)))
        {
            DebugLog($"Skipped Cutscene Segment {msg.SegmentData.Id}");
            FinishCurrentSegment();
            return;
        }

        DebugLog($"Show Cutscene Segment {msg.SegmentData.Id}");
        HidePreviousSegmentStuff();
        _skippable = msg.SegmentData.SegmentType != CutsceneSegmentType.Wait; 
        _currentSegment = AllCutsceneSegments.Create(msg.SegmentData);
        _currentSegment.Start();
    }

    private void Execute(CutsceneFinished msg) => FinishCutscene();

    protected abstract void Execute(SkipCutsceneRequested msg);
    protected abstract void FinishCutscene();
    
    private void HidePreviousSegmentStuff()
    {
        Characters.ForEach(c =>
        {
            c.SetTalkingState(false);
            c.SpeechBubble.ForceHide();
        });
    }
    
    private void FinishCurrentSegment()
    {
        if (_finishTriggered)
            return;
        
        Characters.ForEach(c => c.SetTalkingState(false));
        DebugLog("Segment Finished");
        Message.Publish(new Finished<ShowCutsceneSegment>());
    }

    protected void DebugLog(string msg)
    {
        if (_debugLoggingEnabled)
            Log.Info("Cutscene Presenter - " + msg);
    }
    
    protected abstract void Execute(WinBattleWithRewards msg);
}