using UnityEngine;

public class BoolVariable : ScriptableObject
{
    [SerializeField] private bool value = false;
    [SerializeField] private bool logChanges = false;

    public bool Value
    {
        get => value;
        set
        {
            this.value = value;
            if (logChanges)
                Log.Info($"{name} changed to {value}");
        }
    }

    public bool SetValue(bool newValue) => value = newValue;
}
