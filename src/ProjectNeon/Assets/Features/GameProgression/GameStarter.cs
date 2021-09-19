using UnityEngine;

public class GameStarter : OnMessage<StartNewGame, ContinueCurrentGame, StartNewGameRequested>
{
    [SerializeField] private Navigator navigator;
    [SerializeField] private SaveLoadSystem io;
    [SerializeField] private CurrentAdventure currentAdventure;
    [SerializeField] private AdventureProgress2 adventureProgress2;
    [SerializeField] private Adventure defaultAdventure;
    
    protected override void Execute(StartNewGame msg)
    {
        io.ClearCurrentSlot();
        AllMetrics.SetRunId(CurrentGameData.Data.RunId);
        io.SetShouldShowTutorials(msg.ShouldShowTutorials);
        SelectDefaultAdventure();
    }

    private void SelectDefaultAdventure()
    {
        var adventure = defaultAdventure;
        currentAdventure.Adventure = adventure;
        if (currentAdventure.Adventure.IsV2)
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
