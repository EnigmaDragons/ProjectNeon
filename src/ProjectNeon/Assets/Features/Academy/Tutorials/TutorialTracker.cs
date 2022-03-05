using System.Collections.Generic;
using System.Linq;

public static class TutorialTracker
{
    public static bool ShouldShowTutorial(string tutorialName)
    {
        var tutorialState = CurrentAcademyData.Data.TutorialData;
        return tutorialState.CompletedTutorialNames.None(t => t.Equals(tutorialName));
    }

    public static void RecordTutorialCompleted(string tutorialName)
    {
        CurrentAcademyData.Write(s =>
        {
            s.TutorialData = new AcademyTutorialData
            {
                CompletedTutorialNames = new HashSet<string>(s.TutorialData.CompletedTutorialNames) {tutorialName}.ToArray()
            };
            return s;
        });
        
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
