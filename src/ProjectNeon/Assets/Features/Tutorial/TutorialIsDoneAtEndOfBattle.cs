public class TutorialIsDoneAtEndOfBattle : OnMessage<TutorialWon>
{
    protected override void Execute(TutorialWon msg)
    {
        CurrentAcademyData.Write(a =>
        {
            a.TutorialData = new AcademyTutorialData { CompletedTutorialNames = AcademyData.RequiredLicenseTutorials.ToArray() };
            return a;
        });
        CurrentGameData.Clear();
    }
}