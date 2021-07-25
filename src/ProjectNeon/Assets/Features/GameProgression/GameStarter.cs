using UnityEngine;

public class GameStarter : OnMessage<StartNewGame, ContinueCurrentGame, StartNewGameRequested>
{
    [SerializeField] private Navigator _navigator;
    [SerializeField] private SaveLoadSystem io;
    
    protected override void Execute(StartNewGame msg)
    {
        io.ClearCurrentSlot();
        AllMetrics.SetRunId(CurrentGameData.Data.RunId);
        _navigator.NavigateToAdventureSelection();
    }

    protected override void Execute(ContinueCurrentGame msg)
    {
        if (io.HasSavedGame)
        {
            var phase = io.LoadSavedGame();
            if (phase == CurrentGamePhase.NotStarted)
                _navigator.NavigateToAdventureSelection();
            else if (phase == CurrentGamePhase.SelectedAdventure)
                _navigator.NavigateToSquadSelection();
            else if (phase == CurrentGamePhase.SelectedSquad)
                _navigator.NavigateToGameScene();
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
            Message.Publish(new StartNewGame());
        else
            Message.Publish(new ShowTwoChoiceDialog
            {
                UseDarken = true,
                Prompt = "Starting a new game will abandon your current run. Are you sure you wish to start to start a new game?",
                PrimaryButtonText = "Yes",
                PrimaryAction = () => Message.Publish(new StartNewGame()),
                SecondaryButtonText = "No",
                SecondaryAction = () => { }
            });
    }
}
