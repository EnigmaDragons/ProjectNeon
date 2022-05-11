using UnityEngine;

public class ClearAcademyDataCheat : MonoBehaviour
{
    public void Execute()
    {
        CurrentAcademyData.Clear();
        Message.Publish(new CheatAcceptedSuccessfully());
    }
}
