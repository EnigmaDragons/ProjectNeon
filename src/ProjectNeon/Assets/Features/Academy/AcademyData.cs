using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class AcademyDataSnapshot
{
    public bool IsLicensedBenefactor { get; private set; }
    public List<string> CompletedTutorials { get; private set; }
    public bool HasConfiguredSettings { get; private set; }
    public bool HasCompletedWelcomeToMetroplexCutscene { get; private set; }
    public bool ConfirmedStorySkipBehavior { get; private set; }
    public bool ReceivedNoticeAboutGeneralStarterCards { get; private set; }

    public AcademyDataSnapshot(AcademyData src)
    {
        IsLicensedBenefactor = src.IsLicensedBenefactor;
        CompletedTutorials = src.TutorialData.CompletedTutorialNames.ToList();
        HasConfiguredSettings = src.HasConfiguredSettings;
        HasCompletedWelcomeToMetroplexCutscene = src.HasCompletedWelcomeToMetroplexCutscene;
        ConfirmedStorySkipBehavior = src.ConfirmedStorySkipBehavior;
        ReceivedNoticeAboutGeneralStarterCards = src.ReceivedNoticeAboutGeneralStarterCards;
    }
}

[Serializable]
public class AcademyData
{
    public AcademyTutorialData TutorialData = new AcademyTutorialData();
    public bool HasConfiguredSettings = false;
    public bool HasCompletedWelcomeToMetroplexCutscene = false;
    public bool ConfirmedStorySkipBehavior = false;
    public bool ReceivedNoticeAboutGeneralStarterCards = false;
    
    public static List<string> RequiredLicenseTutorials = new List<string>
    {
        Tutorials.Card,
        Tutorials.DeckBuilder,
        Tutorials.BattleV4
    };

    public static List<string> SimpleTutorialPanels = new List<string>()
    {
        Tutorials.SimpleClinic,
        Tutorials.SimpleDeckbuilder,
        Tutorials.SimpleCardShop,
    };
    
    public bool IsLicensedBenefactor => RequiredLicenseTutorials.All(TutorialData.CompletedTutorialNames.Contains);
    public AcademyDataSnapshot ToSnapshot() => new AcademyDataSnapshot(this);
}

[Serializable]
public class AcademyTutorialData
{
    public string[] CompletedTutorialNames = Array.Empty<string>();
}
