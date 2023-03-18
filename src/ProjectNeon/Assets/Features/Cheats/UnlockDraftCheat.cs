using UnityEngine;

public class UnlockDraftCheat : MonoBehaviour
{
    [SerializeField] private Adventure firstStoryAdventure;

    public void Execute()
    {
        CurrentProgressionData.RecordCompletedAdventure(firstStoryAdventure.id, 0, -1, new int[0]);
        Message.Publish(new CheatAcceptedSuccessfully());
        Message.Publish(new RefreshDraftButtonAccess());
    }
}
