using System.Collections.Generic;
using System.Linq;

public static class TutorialTracker
{
    public static bool ShouldShowTutorial(string tutorialName)
    {
        var tutorialState = CurrentGameData.Data.TutorialData;
        return tutorialState.ShouldShowTutorials &&
               tutorialState.CompletedTutorialNames.None(t => t.Equals(tutorialName));
    }

    public static void RecordTutorialCompleted(string tutorialName)
    {
        CurrentGameData.Write(s =>
        {
            s.TutorialData = new GameTutorialData
            {
                ShouldShowTutorials = s.TutorialData.ShouldShowTutorials,
                CompletedTutorialNames = new HashSet<string>(s.TutorialData.CompletedTutorialNames) { tutorialName }.ToArray()
            };
            return s;
        });
    }
}
