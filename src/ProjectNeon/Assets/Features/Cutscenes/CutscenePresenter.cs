using System.Collections.Generic;
using System.Linq;
using Features.GameProgression;
using UnityEngine;

public class CutscenePresenter : MonoBehaviour
{
    [SerializeField] private Navigator navigator;
    [SerializeField] private CurrentAdventureProgress progress;
    [SerializeField] private FloatReference cutsceneFinishNavigationDelay = new FloatReference(1f);
    [SerializeField] private FloatReference dialogueWaitDelay = new FloatReference(2f);
    [SerializeField] private CurrentCutscene cutscene;
    [SerializeField] private GameObject settingParent;
    [SerializeField] private CutsceneCharacter narrator;
    [SerializeField] private SpawnPartyToMarkers setupParty;

    private CutsceneSegment _currentSegment;
    private bool _debugLoggingEnabled = true;
    private bool _finishTriggered = false;
    private bool _waitFinishTriggered = false;

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
        Message.Subscribe<CutsceneWaitRequested>(Execute, this);
        Message.Subscribe<FinishCutsceneWaitEarlyRequested>(Execute, this);
    }
    
    private void Start()
    {
        _characters.Clear();
        _characters.Add(narrator);
        
        cutscene.Current.Setting.SpawnTo(settingParent);
        setupParty.Execute(settingParent);
        
        var characters = settingParent.GetComponentsInChildren<CutsceneCharacter>();
        characters.Where(c => c.IsInitialized).ForEach(c => _characters.Add(c));
        
        DebugLog($"Characters in cutscene: {string.Join(", ", _characters.Select(c => c.PrimaryName))}");
        
        DebugLog($"Num Cutscene Segments {cutscene.Current.Segments.Length}");
        MessageGroup.Start(
            new MultiplePayloads(cutscene.Current.Segments.Select(s => new ShowCutsceneSegment(s)).Cast<object>().ToArray()), 
            () => Message.Publish(new CutsceneFinished()));
    }

    private void Execute(CutsceneWaitRequested msg)
    {
        _waitFinishTriggered = false;
        this.ExecuteAfterDelay(msg.Duration, FinishWait);
    }
    
    private void Execute(FinishCutsceneWaitEarlyRequested msg) => FinishWait();

    private void FinishWait()
    {
        if (_waitFinishTriggered)
            return;

        _waitFinishTriggered = true;
        FinishCurrentSegment();
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
    
    private void Execute(ShowCharacterDialogueLine msg)
    {
        DebugLog($"Show Character Dialogue Line {msg.CharacterAlias}");
        _characters.FirstOrMaybe(c => c.Matches(msg.CharacterAlias))
            .ExecuteIfPresentOrElse(
                c => c.SpeechBubble.Display(msg.Text, shouldAutoProceed: true, manualInterventionDisablesAuto: false, 
                    () => this.ExecuteAfterDelay(FinishCurrentSegment, dialogueWaitDelay)),
                () =>
                {
                    DebugLog($"Character Not Found in Cutscene {msg.CharacterAlias}");
                    FinishCurrentSegment();
                });
    }

    private void Execute(ShowCutsceneSegment msg)
    {
        DebugLog("Show Cutscene Segment");
        HidePreviousSegmentStuff();
        _currentSegment = AllCutsceneSegments.Create(msg.SegmentData);
        _currentSegment.Start();
    }

    private void Execute(CutsceneFinished msg)
    {
        if (_finishTriggered)
            return;
        
        HidePreviousSegmentStuff();
        DebugLog("Cutscene Finished");
        _finishTriggered = true;
        progress.AdventureProgress.Advance();
        this.ExecuteAfterDelay(navigator.NavigateToGameSceneV4, cutsceneFinishNavigationDelay);
    }

    private void HidePreviousSegmentStuff()
    {
        _characters.ForEach(c => c.SpeechBubble.ForceHide());
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
