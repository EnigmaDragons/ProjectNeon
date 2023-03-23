using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CutscenePresenter : BaseCutscenePresenter
{
    [SerializeField] private Navigator navigator;
    [SerializeField] private CurrentAdventure adventure;
    [SerializeField] private AdventureConclusionState conclusion;
    [SerializeField] private FloatReference cutsceneFinishNavigationDelay = new FloatReference(1f);
    [SerializeField] private GameObject defaultCamera;
    [SerializeField] private GameObject settingParent;
    [SerializeField] private SpawnPartyToMarkers setupParty;
    [SerializeField] private PartyAdventureState partyState;
    [SerializeField] private GameObject[] disableOnFinished;
    [SerializeField] private CurrentBoss boss;
    [SerializeField] private CurrentMapSegmentV5 map;

    private bool _skipDelay;
    
    private void Start()
    {
        if (cutscene.Current == null)
        {
            Log.Error("Null: CurrentCutscene.Current");
            Message.Publish(new CutsceneFinished());
            return;
        }

        if (cutscene.Current.Setting == null)
        {
            Log.Error($"Null: Cutscene Setting for {cutscene.Current.name}");
            Message.Publish(new CutsceneFinished());
            return;
        }

        try
        {
            InitBasicCharacters();
            cutscene.Current.Setting.SpawnTo(settingParent);
            setupParty.Execute(settingParent);
            InitCameras();
            InitSettingCharacters();

            DebugLog($"Characters in cutscene: {string.Join(", ", Characters.Select(c => c.PrimaryName))}");
            DebugLog($"Num Cutscene Segments {cutscene.Current.Segments.Length}");
            MessageGroup.TerminateAndClear();
            MessageGroup.Start(
                new MultiplePayloads("Cutscene Script",
                    cutscene.Current.Segments.Select(s => new ShowCutsceneSegment(s)).Cast<object>().ToArray()),
                () => Message.Publish(new CutsceneFinished()));
        }
        catch (Exception e)
        {
            Log.Error("Still got the Null Ref in Cutscene Presenter");
            Log.Error(e);
            Message.Publish(new CutsceneFinished());
        }
    }

    private void InitCameras()
    {
        var cameras = settingParent.GetComponentsInChildren<Camera>();
        if (cameras.Any())
            defaultCamera.SetActive(false);
    }

    private void InitSettingCharacters()
    {
        var characters = settingParent.GetComponentsInChildren<CutsceneCharacter>();
        characters.Where(c => c.IsInitialized).ForEach(c => Characters.Add(c));
        settingParent.GetComponentsInChildren<CutsceneCharacterAdditionalVisual>()
            .ForEach(v => v.OwnerAliases.ForEach(a =>
            {
                if (CharacterAdditionalVisuals.TryGetValue(a, out var items))
                    items.Add(v.gameObject);
                else
                    CharacterAdditionalVisuals[a] = new List<GameObject> { v.gameObject };
            }));
    }

    private void InitBasicCharacters()
    {
        Characters.Clear();
        Characters.Add(narrator);
        Characters.Add(you);
        Characters.Add(player);
    }

    protected override void Execute(SkipCutsceneRequested msg) => SkipCutscene();

    private void SkipCutscene()
    {
        cutscene.Current.MarkSkipped(progress.AdventureProgress);
        _skipDelay = true;
        FinishCutscene();
    }

    protected override void FinishCutscene()
    {
        if (_finishTriggered)
            return;
        
        _finishTriggered = true;
        DebugLog("Cutscene Finished");
        narrator.SpeechBubble.ForceHide();
        you.SpeechBubble.ForceHide();
        player.SpeechBubble.ForceHide();
        disableOnFinished.ForEach(d => d.SetActive(false));
        MessageGroup.TerminateAndClear();

        var shouldGoToAdventureVictoryScreen = cutscene.OnCutsceneFinishedAction.IsMissing
                                               && cutscene.Current.IsPrimaryCutscene
                                               && progress.AdventureProgress.IsFinalStageSegment;
        if (cutscene.OnCutsceneFinishedAction.IsMissing) // Is Game Flow Cutscene
        {
            if (progress.AdventureProgress.AdventureType == GameAdventureProgressType.V4)
                progress.AdventureProgress.Advance();
            else if (cutscene.Current.IsPrimaryCutscene)
                progress.AdventureProgress.Advance();
            else
                map.DisableSavingCurrentNode();
            Message.Publish(new AutoSaveRequested());
        }
        
        var onFinishedAction = shouldGoToAdventureVictoryScreen 
            ? () => GameWrapup.NavigateToVictoryScreen(progress, adventure, boss, navigator, conclusion, partyState.Heroes) 
            : cutscene.OnCutsceneFinishedAction.Select(a => a, NavigateToInferredGameScene);
        if (shouldGoToAdventureVictoryScreen && !CurrentAcademyData.Data.IsLicensedBenefactor)
            onFinishedAction = () => TutorialWonHandler.Execute();
        if (_skipDelay)
            onFinishedAction();
        else
            this.ExecuteAfterDelay(onFinishedAction, cutsceneFinishNavigationDelay);
    }

    protected override void Execute(WinBattleWithRewards msg) => SkipCutscene();

    private void NavigateToInferredGameScene()
    {
        var type = progress.AdventureProgress.AdventureType;
        if (type == GameAdventureProgressType.V2)
            navigator.NavigateToGameScene();
        else if (type == GameAdventureProgressType.V4)
            navigator.NavigateToGameSceneV4();
        else if (type == GameAdventureProgressType.V5)
            navigator.NavigateToGameSceneV5();
        else
            Log.Error("Unable to infer Cutscene Finished Scene to Navigate to");
    }
}
