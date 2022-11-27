using I2.Loc;
using UnityEngine;

public class UpdateLocalizationData : MonoBehaviour
{
    private static bool IsUpdating = false;
    private static readonly LanguageSource.fnOnSourceUpdated OnFinished = (_, __, ___) =>
    {
        Log.Info("Finished Translation Data Update");
        LocalizationManager.SetLanguageAndCode(LocalizationManager.CurrentLanguage,
            LocalizationManager.CurrentLanguageCode, true, true);
        IsUpdating = false;
        Message.Publish(new CheatAcceptedSuccessfully());
    };
    
    public void Go()
    {
        if (IsUpdating)
            return;

        IsUpdating = true;
        LocalizationManager.ForceUpdateLocalizationData(OnFinished);
        Message.Publish(new CheatAcceptedSuccessfully());
    }
}
