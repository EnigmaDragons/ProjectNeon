using UnityEngine;

public class CustomNavigateButton : OnMessage<AcademyDataUpdated>
{
    [SerializeField] private Navigator navigator;
    [SerializeField] private TextCommandButton button;
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
            button.Init("Proceed", navigator.NavigateToTitleScreen);
        else
            button.Init("Proceed", cutscene.Execute);
        if (!wasActive)
            button.Hide();
    }
}
