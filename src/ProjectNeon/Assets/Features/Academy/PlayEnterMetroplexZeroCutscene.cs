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
            CurrentAcademyData.Mutate(a => a.HasCompletedWelcomeToMetroplexCutscene = true);
            navigator.NavigateToTitleScreen();
        })));
    }
}
