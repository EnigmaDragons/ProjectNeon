using UnityEngine;

public class ClearAcademyDataCheat : MonoBehaviour
{
    public void Execute()
    {
        CurrentAcademyData.Clear();
        CurrentGameData.Clear();
        Message.Publish(new CheatAcceptedSuccessfully());
    }
}
