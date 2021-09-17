using UnityEngine;

public class LoadGameOnStart : MonoBehaviour
{
    private void Start()
    {
        if (CurrentGameData.HasActiveGame)
            Message.Publish(new ContinueCurrentGame());
        else
            Log.Error("Cannot launch without an active game in progress. Start a New Game from the Title Screen before launching.");
    }
}
