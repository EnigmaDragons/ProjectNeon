using UnityEngine;

public class FinishTutorialCheat : MonoBehaviour
{
    public void Execute()
    {
        Message.Publish(new CheatAcceptedSuccessfully());
        TutorialWonHandler.Execute();
    }
}
