using UnityEngine;

public class ClearProgressionDataCheat : MonoBehaviour
{
    public void Execute()
    {
        CurrentProgressionData.Clear();
        Message.Publish(new CheatAcceptedSuccessfully());
    }
}
