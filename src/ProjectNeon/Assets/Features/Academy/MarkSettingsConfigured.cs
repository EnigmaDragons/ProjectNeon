using UnityEngine;

public class MarkSettingsConfigured : MonoBehaviour
{
    public void Execute()
    {
        CurrentAcademyData.Mutate(a => a.HasConfiguredSettings = true);
    }
}