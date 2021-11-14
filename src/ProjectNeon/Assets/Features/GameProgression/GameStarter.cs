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
    [SerializeField] private Adventure defaultAdventure;
    [SerializeField] private bool allowPlayerToSelectAdventure;
    
    protected override void Execute(StartNewGame msg)
    {
        io.ClearCurrentSlot();
        AllMetrics.SetRunId(CurrentGameData.Data.RunId);
        io.SetShouldShowTutorials(msg.ShouldShowTutorials);
        if (allowPlayerToSelectAdventure)
            navigator.NavigateToAdventureSelection();
        else
            SelectDefaultAdventure();
    }
    
    private void SelectDefaultAdventure()
    {
        var adventure = defaultAdventure;
        if (adventure.IsV2)
            adventureProgress.AdventureProgress = adventureProgress2;
        if (adventure.IsV4)
            adventureProgress.AdventureProgress = adventureProgress4;
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
