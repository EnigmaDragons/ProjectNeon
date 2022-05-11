using System;
using UnityEngine;

public class PlayEnterMetroplexZeroCutscene : MonoBehaviour
{
    [SerializeField] private Cutscene cutscene;
    [SerializeField] private Navigator navigator;
    
    public void Execute()
    {
        Message.Publish(new StartCutsceneRequested(cutscene, Maybe<Action>.Present(() =>
        {
            CurrentAcademyData.Write(a =>
            {
                a.HasCompletedWelcomeToMetroplexCutscene = true;
                return a;
            });
            navigator.NavigateToTitleScreen();
        })));
    }
}
