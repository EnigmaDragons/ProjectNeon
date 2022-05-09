using UnityEngine;

public class MarkSettingsConfigured : MonoBehaviour
{
    public void Execute()
    {
        CurrentAcademyData.Write(a =>
        {
            a.HasConfiguredSettings = true;
            return a;
        });
    }
}