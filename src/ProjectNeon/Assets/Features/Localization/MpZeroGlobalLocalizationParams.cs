using I2.Loc;
using UnityEngine;

public class MpZeroGlobalLocalizationParams : MonoBehaviour, ILocalizationParamsManager
{
    private void Awake() => OnEnable(); //race condition
    
    public void OnEnable()
    {
        if (LocalizationManager.ParamManagers.Contains(this)) return;
        
        LocalizationManager.ParamManagers.Add(this);
        LocalizationManager.LocalizeAll(true);
    }

    public void OnDisable()
    {
        LocalizationManager.ParamManagers.Remove(this);
    }

    public virtual string GetParameterValue(string paramName)
    {
        if (paramName.StartsWith("t:"))
            return paramName.Substring(2).ToLocalized();
        if (paramName.Equals("SaveGameVersion"))
            return CurrentGameData.SaveGameVersion;
        if (paramName.Equals("GameVersion"))
            return CurrentGameData.GameVersion;
        return paramName.FromI2ParamValue();
    }
}

#if UNITY_EDITOR
public class MpZeroEditorGlobalLocalizationParams : ILocalizationParamsManager
{
    public virtual string GetParameterValue(string paramName)
    {
        if (paramName.StartsWith("t:"))
            return paramName.Substring(2).ToLocalized();
        if (paramName.Equals("SaveGameVersion"))
            return CurrentGameData.SaveGameVersion;
        if (paramName.Equals("GameVersion"))
            return CurrentGameData.GameVersion;
        return paramName.FromI2ParamValue();
    }
}
#endif
