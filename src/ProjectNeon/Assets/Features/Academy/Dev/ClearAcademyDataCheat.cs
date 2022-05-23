using UnityEngine;

public class ClearAcademyDataCheat : MonoBehaviour
{
    public void Execute()
    {
        CurrentAcademyData.Clear();
        if (CurrentGameData.HasActiveGame)
        {
            var data = CurrentGameData.Data;
            if (data.AdventureProgress.AdventureId == 10) // Tutorial Adventure ID
                CurrentGameData.Clear();
        }
        Message.Publish(new CheatAcceptedSuccessfully());
    }
}
