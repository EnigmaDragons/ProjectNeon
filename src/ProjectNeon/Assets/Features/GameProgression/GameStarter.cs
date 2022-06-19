using System.Linq;
using UnityEngine;

public class GameStarter : OnMessage<StartNewGame, ContinueCurrentGame, StartNewGameRequested, StartAdventureV5Requested>
{
    [SerializeField] private Navigator navigator;
    [SerializeField] private SaveLoadSystem io;
    [SerializeField] private CurrentAdventure currentAdventure;
    [SerializeField] private CurrentAdventureProgress adventureProgress;
    [SerializeField] private AdventureProgress2 adventureProgress2;
    [SerializeField] private AdventureProgressV4 adventureProgress4;
    [SerializeField] private AdventureProgressV5 adventureProgress5;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private Adventure defaultAdventure;
    [SerializeField] private bool allowPlayerToSelectAdventure;
    [SerializeField] private EncounterBuilderHistory encounterHistory;
    
    protected override void Execute(StartNewGame msg)
    {
        Init();

        if (allowPlayerToSelectAdventure)
            navigator.NavigateToAdventureSelection();
        else if (defaultAdventure.IsV5)
            StartDefaultAdventureV5();
        else if (defaultAdventure.IsV2)
            SelectDefaultAdventureV2();
        else if (defaultAdventure.IsV4)
            StartDefaultAdventureV4(true);
    }

    private void Init()
    {
        io.ClearCurrentSlot();
        encounterHistory.Clear();
        AllMetrics.SetRunId(CurrentGameData.Data.RunId);
        RunTimer.ConsumeElapsedTime();
    }

    private void SelectDefaultAdventureV2()
    {
        var adventure = defaultAdventure;
        adventureProgress.AdventureProgress = adventureProgress2;
        adventureProgress.AdventureProgress.Init(adventure, 0);
        CurrentGameData.Write(s =>
        {
            s.IsInitialized = true;
            s.Phase = CurrentGamePhase.SelectedAdventure;
            s.AdventureProgress = adventureProgress.AdventureProgress.GetData();
            return s;
        });
        navigator.NavigateToSquadSelection();
    }

    private void StartDefaultAdventureV4(bool playerIsPro)
    {
        var adventure = defaultAdventure;
        var startingSegment = playerIsPro ? adventure.StagesV4.First().RepeatPlayStartingSegmentIndex : 0;
        adventureProgress.AdventureProgress = adventureProgress4;
        adventureProgress.AdventureProgress.Init(adventure, 0, startingSegment);
        if (adventure.FixedStartingHeroes.Length == 0)
        {
            Log.Error("Developer Data Error - V4 Adventures should start with a fixed party");
            navigator.NavigateToSquadSelection();
        }
        else
        {
            party.Initialized(adventure.FixedStartingHeroes);
            CurrentGameData.Write(s =>
            {
                s.IsInitialized = true;
                s.Phase = CurrentGamePhase.SelectedSquad;
                s.AdventureProgress = adventureProgress.AdventureProgress.GetData();
                s.PartyData = party.GetData();
                return s;
            });
            navigator.NavigateToGameSceneV4();
        }
    }

    private void StartDefaultAdventureV5() => StartAdventureV5(defaultAdventure);
    
    protected override void Execute(StartAdventureV5Requested msg)
    {
        Init();
        StartAdventureV5(msg.Adventure);
    }

    private void StartAdventureV5(Adventure adventure)
    {
        var startingSegment = 0;
        adventureProgress.AdventureProgress = adventureProgress5;
        adventureProgress.AdventureProgress.Init(adventure, 0, startingSegment);
        party.Initialized(adventure.FixedStartingHeroes);
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
                    Message.Publish(new ShowInfoDialog(
                        $"Load failed. Save Game Version is {CurrentGameData.SaveGameVersion}. Current Game Version is {CurrentGameData.GameVersion}. Updating your game may fix this issue.",
                        "Null Persp"));
                }
                else
                {
                    io.ClearCurrentSlot();
                    CurrentGameData.Clear();
                    Message.Publish(new RefreshMainMenu());
                    Message.Publish(new ShowInfoDialog(
                        "Unfortunately, your Save Game was unable to be loaded. A bug report has been automatically filed.",
                        "Drek!"));
                }
            }
        }
    }

    protected override void Execute(StartNewGameRequested msg)
    {
        if (!CurrentGameData.HasActiveGame) 
            KickOffGameStartProcess();
        else
            Message.Publish(new ShowTwoChoiceDialog
            {
                UseDarken = true,
                Prompt = "Starting a new game will abandon your current run. Are you sure you wish to start to start a new game?",
                PrimaryButtonText = "Yes",
                PrimaryAction = KickOffGameStartProcess,
                SecondaryButtonText = "No",
                SecondaryAction = () => { }
            });
    }

    private void KickOffGameStartProcess()
    {
        Message.Publish(new StartNewGame());
    }
}
