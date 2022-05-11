using UnityEngine;

public class FinishAcademyCheat : MonoBehaviour
{
    public void Execute()
    {
        CurrentAcademyData.Skip();
        Message.Publish(new CheatAcceptedSuccessfully());
    }
}
