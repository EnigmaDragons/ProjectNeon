using System.Collections.Generic;
using System.Linq;

public static class TutorialTracker
{
    public static bool ShouldShowTutorial(string tutorialName)
    {
        var tutorialState = CurrentAcademyData.Data.TutorialData;
        var shouldShow = tutorialState.CompletedTutorialNames.None(t => t.Equals(tutorialName));
        Log.Info($"Should Show Tutorial {tutorialName} - {shouldShow}");
        return shouldShow;
    }

    public static void RecordTutorialCompleted(string tutorialName)
    {
        CurrentAcademyData.Mutate(s => s.TutorialData = new AcademyTutorialData
        {
            CompletedTutorialNames = new HashSet<string>(s.TutorialData.CompletedTutorialNames) {tutorialName}.ToArray()
        });
    }
}
