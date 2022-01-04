using System.Linq;
using Features.GameProgression;
using UnityEngine;

public class GameStarter : OnMessage<StartNewGame, ContinueCurrentGame, StartNewGameRequested>
{
    [SerializeField] private Navigator navigator;
    [SerializeField] private SaveLoadSystem io;
    [SerializeField] private CurrentAdventure currentAdventure;
    [SerializeField] private CurrentAdventureProgress adventureProgress;
    [SerializeField] private AdventureProgress2 adventureProgress2;
    [SerializeField] private AdventureProgressV4 adventureProgress4;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private Adventure defaultAdventure;
    [SerializeField] private bool allowPlayerToSelectAdventure;
    [SerializeField] private EncounterBuilderHistory encounterHistory;
    
    protected override void Execute(StartNewGame msg)
    {
        io.ClearCurrentSlot();
        encounterHistory.Clear();
        AllMetrics.SetRunId(CurrentGameData.Data.RunId);
        io.SetShouldShowTutorials(msg.ShouldShowTutorials);
        
        if (defaultAdventure.IsV2)
            if (allowPlayerToSelectAdventure)
                navigator.NavigateToAdventureSelection();
            else
                SelectDefaultAdventureV2();
        
        if (defaultAdventure.IsV4)
            StartDefaultAdventureV4(!msg.ShouldShowTutorials);
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
        Message.Publish(new GameStarted());
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
            Message.Publish(new GameStarted());
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
            Message.Publish(new GameStarted());
            navigator.NavigateToGameSceneV4();
        }
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
            }
            else if (phase == CurrentGamePhase.LoadError)
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
        if (defaultAdventure.IsV2 || defaultAdventure.IsV4)
            AskPlayerWhetherTutorialsShouldBeEnabled();
        else
            Message.Publish(new StartNewGame(false));
    }
    
    private void AskPlayerWhetherTutorialsShouldBeEnabled()
    {
        Message.Publish(new ShowTwoChoiceDialog
        {
            UseDarken = true,
            Prompt = "Is this your first time playing Metroplex Zero?",
            PrimaryButtonText = "Yes",
            PrimaryAction = () => Message.Publish(new StartNewGame(true)),
            SecondaryButtonText = "No",
            SecondaryAction = () => Message.Publish(new StartNewGame(false)),
        });
    }
}
