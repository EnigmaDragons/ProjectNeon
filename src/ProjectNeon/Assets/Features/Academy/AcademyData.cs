using System;

[Serializable]
public class AcademyData
{
    public AcademyTutorialData TutorialData = new AcademyTutorialData();
}

[Serializable]
public class AcademyTutorialData
{
    public string[] CompletedTutorialNames = Array.Empty<string>();
}
