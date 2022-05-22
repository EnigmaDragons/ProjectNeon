public class TutorialIsDoneAtEndOfBattle : OnMessage<BattleFinished>
{
    protected override void Execute(BattleFinished msg)
    {
        CurrentAcademyData.Write(a =>
        {
            a.TutorialData = new AcademyTutorialData { CompletedTutorialNames = AcademyData.RequiredLicenseTutorials.ToArray() };
            return a;
        });
    }
}