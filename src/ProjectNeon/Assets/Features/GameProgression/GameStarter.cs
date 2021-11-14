using UnityEngine;

public class GameStarter : OnMessage<StartNewGame, ContinueCurrentGame, StartNewGameRequested>
{
    [SerializeField] private Navigator navigator;
    [SerializeField] private SaveLoadSystem io;
    [SerializeField] private CurrentAdventure currentAdventure;
    [SerializeField] private Library library;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private AdventureProgress2 adventureProgress2;
    [SerializeField] private AdventureProgressV4 adventureProgressV4;
    [SerializeField] private Adventure defaultAdventure;
    [SerializeField] private bool allowPlayerToSelectAdventure;
    
    protected override void Execute(StartNewGame msg)
    {
        io.ClearCurrentSlot();
        AllMetrics.SetRunId(CurrentGameData.Data.RunId);
        io.SetShouldShowTutorials(msg.ShouldShowTutorials);
        
        if (defaultAdventure.IsV2)
            if (allowPlayerToSelectAdventure)
                navigator.NavigateToAdventureSelection();
            else
                SelectDefaultAdventureV2();
        
        if (defaultAdventure.IsV4)
            StartDefaultAdventureV4();
    }
    
    private void SelectDefaultAdventureV2()
    {
        var adventure = defaultAdventure;
        currentAdventure.Adventure = adventure;
        adventureProgress2.Init();
        CurrentGameData.Write(s =>
        {
            s.IsInitialized = true;
            s.Phase = CurrentGamePhase.SelectedAdventure;
            s.AdventureProgress = adventureProgress2.GetData();
            return s;
        });
        navigator.NavigateToSquadSelection();
    }

    private void StartDefaultAdventureV4()
    {
        var adventure = defaultAdventure;
        currentAdventure.Adventure = adventure;
        adventureProgressV4.Init();

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
                s.PartyData = party.GetData();
                return s;
            });
            navigator.NavigateToGameSceneV4();
        }
    } 

    protected override void Execute(ContinueCurrentGame msg)
    {
        if (io.HasSavedGame)
        {
            var phase = io.LoadSavedGame();
            if (phase == CurrentGamePhase.NotStarted)
                navigator.NavigateToAdventureSelection();
            else if (phase == CurrentGamePhase.SelectedAdventure)
                navigator.NavigateToSquadSelection();
            else if (phase == CurrentGamePhase.SelectedSquad)
                navigator.NavigateToGameScene();
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
            AskPlayerWhetherTutorialsShouldBeEnabled();
        else
            Message.Publish(new ShowTwoChoiceDialog
            {
                UseDarken = true,
                Prompt = "Starting a new game will abandon your current run. Are you sure you wish to start to start a new game?",
                PrimaryButtonText = "Yes",
                PrimaryAction = AskPlayerWhetherTutorialsShouldBeEnabled,
                SecondaryButtonText = "No",
                SecondaryAction = () => { }
            });
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
