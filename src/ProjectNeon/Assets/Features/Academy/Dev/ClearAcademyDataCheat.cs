using UnityEngine;

public class ClearAcademyDataCheat : MonoBehaviour
{
    public void Execute()
    {
        CurrentAcademyData.Clear();
        if (CurrentGameData.HasActiveGame)
            CurrentGameData.Clear();
        Message.Publish(new CheatAcceptedSuccessfully());
    }
}
