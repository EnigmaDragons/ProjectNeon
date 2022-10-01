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
    [SerializeField] private GameObject[] disableOnFinished;

    private bool _skipDelay;
    
    private void Start()
    {
        Characters.Clear();
        Characters.Add(narrator);
        Characters.Add(you);
        Characters.Add(player);

        cutscene.Current.Setting.SpawnTo(settingParent);
        setupParty.Execute(settingParent);

        var cameras = settingParent.GetComponentsInChildren<Camera>();
        if (cameras.Any())
            defaultCamera.SetActive(false);
        
        var characters = settingParent.GetComponentsInChildren<CutsceneCharacter>();
        characters.Where(c => c.IsInitialized).ForEach(c => Characters.Add(c));
        
        DebugLog($"Characters in cutscene: {string.Join(", ", Characters.Select(c => c.PrimaryName))}");
        
        DebugLog($"Num Cutscene Segments {cutscene.Current.Segments.Length}");
        MessageGroup.TerminateAndClear();
        MessageGroup.Start(
            new MultiplePayloads("Cutscene Script", cutscene.Current.Segments.Select(s => new ShowCutsceneSegment(s)).Cast<object>().ToArray()), 
            () => Message.Publish(new CutsceneFinished()));
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
            Message.Publish(new AutoSaveRequested());
        }
        
        var onFinishedAction = shouldGoToAdventureVictoryScreen 
            ? () => GameWrapup.NavigateToVictoryScreen(progress, adventure, navigator, conclusion, setupParty.Party.Heroes.Cast<HeroCharacter>().ToArray()) 
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
