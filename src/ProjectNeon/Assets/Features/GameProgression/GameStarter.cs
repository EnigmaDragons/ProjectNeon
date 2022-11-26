using UnityEngine;

public class GameStarter : OnMessage<StartNewGame, ContinueCurrentGame, StartNewGameRequested, StartAdventureV5Requested>, ILocalizeTerms
{
    [SerializeField] private Navigator navigator;
    [SerializeField] private SaveLoadSystem io;
    [SerializeField] private CurrentAdventure currentAdventure;
    [SerializeField] private CurrentAdventureProgress adventureProgress;
    [SerializeField] private AdventureProgressV5 adventureProgress5;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private Adventure defaultAdventure;
    [SerializeField] private bool allowPlayerToSelectAdventure;
    [SerializeField] private EncounterBuilderHistory encounterHistory;
    [SerializeField] private Library library;
    
    protected override void Execute(StartNewGame msg)
    {
        if (CurrentGameData.HasActiveGame && adventureProgress.HasActiveAdventure && adventureProgress.AdventureProgress.CurrentStageProgress > 0 && adventureProgress.AdventureProgress.AdventureId != 10)
            CurrentProgressionData.Write(x =>
            {
                x.RunsFinished += 1;
                return x;
            });
        Init();

        if (allowPlayerToSelectAdventure)
            navigator.NavigateToAdventureSelection();
        else if (defaultAdventure.IsV5)
            StartDefaultAdventureV5();
        else if (defaultAdventure.IsV2)
            Log.Error("V2 Adventures No Longer Supported");
        else if (defaultAdventure.IsV4)
            Log.Error("V4 Adventures No Longer Supported");
    }

    private void Init()
    {
        io.ClearCurrentSlot();
        encounterHistory.Clear();
        AllMetrics.SetRunId(CurrentGameData.Data.RunId);
        RunTimer.ConsumeElapsedTime();
    }
    
    private void StartDefaultAdventureV5() => StartAdventureV5(defaultAdventure, Maybe<BaseHero[]>.Missing(), library.DefaultDifficulty);
    
    protected override void Execute(StartAdventureV5Requested msg)
    {
        Init();
        StartAdventureV5(msg.Adventure, msg.OverrideHeroes, msg.Difficulty);
    }

    private void StartAdventureV5(Adventure adventure, Maybe<BaseHero[]> overrideHeroes, Maybe<Difficulty> possibleDifficulty)
    {
        var startingSegment = 0;
        var difficulty = possibleDifficulty.IsPresent ? possibleDifficulty.Value : library.DefaultDifficulty;
        adventureProgress.AdventureProgress = adventureProgress5;
        adventureProgress.AdventureProgress.Init(adventure, 0, startingSegment);
        adventureProgress.AdventureProgress.Difficulty = difficulty;
        foreach (var globalEffect in difficulty.GlobalEffects)
            adventureProgress.AdventureProgress.GlobalEffects.Apply(globalEffect, new GlobalEffectContext(adventureProgress.AdventureProgress.GlobalEffects));
        if (adventure.Mode == AdventureMode.Draft)
            party.InitializedForDraft(overrideHeroes.Select(o => o, adventure.FixedStartingHeroes));
        else
            party.Initialized(overrideHeroes.Select(o => o, adventure.FixedStartingHeroes));
        party.UpdateClinicVouchersBy(adventure.StartingClinicVouchers);
        CurrentGameData.Write(s =>
        {
            s.IsInitialized = true;
            s.Phase = CurrentGamePhase.SelectedSquad;
            s.AdventureProgress = adventureProgress.AdventureProgress.GetData();
            s.PartyData = party.GetData();
            return s;
        });
        Navigate(navigator, adventureProgress5);
    }

    public static void Navigate(Navigator n, AdventureProgressV5 a)
    {
        var segment = a.CurrentStageSegment;
        DevLog.Info($"Start Adventure. Auto-Start: {segment.ShouldAutoStart}. MapNodeType: {segment.MapNodeType}");
        if (a != null && segment.ShouldAutoStart && segment.MapNodeType != MapNodeType.Unknown)
            segment.Start();
        else
            n.NavigateToGameSceneV5();
    }

    protected override void Execute(ContinueCurrentGame msg)
    {
        if (io.HasSavedGame)
        {
            encounterHistory.Clear();
            var phase = io.LoadSavedGame();
            Message.Publish(new GameLoaded());
            if (phase == CurrentGamePhase.NotStarted)
                navigator.NavigateToAdventureSelection();
            else if (phase == CurrentGamePhase.SelectedAdventure)
                navigator.NavigateToSquadSelection();
            else if (phase == CurrentGamePhase.SelectedSquad)
            {
                var adventureType = CurrentGameData.Data.AdventureProgress.Type;
                if (adventureType == GameAdventureProgressType.V2)
                    navigator.NavigateToGameScene();
                if (adventureType == GameAdventureProgressType.V4)
                    navigator.NavigateToGameSceneV4();
                if (adventureType == GameAdventureProgressType.V5)
                    navigator.NavigateToGameSceneV5();
            }
            else if (phase == CurrentGamePhase.LoadError)
            {
                if (!CurrentGameData.SaveMatchesCurrentVersion)
                {
                    Message.Publish(new RefreshMainMenu());
                    Message.Publish(ShowLocalizedDialog.Info(DialogTerms.LoadFailedVersionMismatch, DialogTerms.OptionDrek));
                }
                else
                {
                    io.ClearCurrentSlot();
                    CurrentGameData.Clear();
                    Message.Publish(new RefreshMainMenu());
                    Message.Publish(ShowLocalizedDialog.Info(DialogTerms.LoadFailed, DialogTerms.OptionDrek));
                }
            }
        }
    }

    protected override void Execute(StartNewGameRequested msg)
    {
        if (!CurrentGameData.HasActiveGame) 
            KickOffGameStartProcess(msg.Mode);
        else
            Message.Publish(new ShowLocalizedDialog
            {
                UseDarken = true,
                PromptTerm = DialogTerms.AbandonCurrentRunWarning,
                PrimaryButtonTerm = DialogTerms.OptionYes,
                PrimaryAction = () => KickOffGameStartProcess(msg.Mode),
                SecondaryButtonTerm = DialogTerms.OptionNo,
                SecondaryAction = () => { }
            });
    }

    private void KickOffGameStartProcess(AdventureMode mode)
    {
        if (mode == AdventureMode.Draft)
            navigator.NavigateToDraftAdventureSelection();
        else
            Message.Publish(new StartNewGame());
    }

    public string[] GetLocalizeTerms() => new[]
    {
        DialogTerms.OptionYes,
        DialogTerms.OptionNo,
        DialogTerms.OptionDrek,
        DialogTerms.AbandonCurrentRunWarning,
        DialogTerms.LoadFailed,
        DialogTerms.LoadFailedVersionMismatch
    };
}
