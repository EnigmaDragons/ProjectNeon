using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class AcademyDataSnapshot
{
    public bool IsLicensedBenefactor { get; private set; }
    public List<string> CompletedTutorials { get; private set; }

    public AcademyDataSnapshot(AcademyData src)
    {
        IsLicensedBenefactor = src.IsLicensedBenefactor;
        CompletedTutorials = src.TutorialData.CompletedTutorialNames.ToList();
    }
}

[Serializable]
public class AcademyData
{
    public AcademyTutorialData TutorialData = new AcademyTutorialData();
    
    public static List<string> RequiredLicenseTutorials = new List<string>
    {
        Tutorials.Card,
        Tutorials.DeckBuilder,
        Tutorials.BattleV4
    };

    public bool IsLicensedBenefactor => RequiredLicenseTutorials.All(TutorialData.CompletedTutorialNames.Contains);
    public AcademyDataSnapshot ToSnapshot() => new AcademyDataSnapshot(this);
}

[Serializable]
public class AcademyTutorialData
{
    public string[] CompletedTutorialNames = Array.Empty<string>();
}
