using UnityEngine;

public class CustomNavigateButton : OnMessage<AcademyDataUpdated>, ILocalizeTerms
{
    [SerializeField] private Navigator navigator;
    [SerializeField] private LocalizedCommandButton button;
    [SerializeField] private PlayEnterMetroplexZeroCutscene cutscene;

    protected override void AfterEnable() => Render();
    
    protected override void Execute(AcademyDataUpdated msg)
    {
        Render();
    }

    private void Render()
    {
        var data = CurrentAcademyData.Data;
        var wasActive = button.gameObject.activeSelf;
        if (data.HasCompletedWelcomeToMetroplexCutscene)
            button.InitTerm("Menu/Proceed", navigator.NavigateToTitleScreen);
        else
            button.InitTerm("Menu/Proceed", cutscene.Execute);
        if (!wasActive)
            button.Hide();
    }

    public string[] GetLocalizeTerms()
        => new[] {"Menu/Proceed"};
}
