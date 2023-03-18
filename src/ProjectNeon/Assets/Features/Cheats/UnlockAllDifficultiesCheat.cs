using UnityEngine;

public class UnlockAllDifficultiesCheat : MonoBehaviour
{
    public void Unlock()
    {
        CurrentProgressionData.RecordCompletedAdventure(9, 99, -1, new [] { 9 });
    }
}