
using UnityEngine;

public abstract class PowerCurve : ScriptableObject
{
    public abstract float GetValue(float t);
    public int GetValueAsInt(float t) => (int) Mathf.Round(GetValue(t));
}
