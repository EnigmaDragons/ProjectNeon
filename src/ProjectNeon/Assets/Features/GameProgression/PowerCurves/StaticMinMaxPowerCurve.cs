using UnityEngine;

[CreateAssetMenu(menuName="Adventure/Power Curve")]
public class StaticMinMaxPowerCurve : ScriptableObject
{
    [SerializeField] private ParticleSystem.MinMaxCurve curve;

    public float GetValue(float t) => curve.Evaluate(t);
    public int GetValueAsInt(float t) => (int) Mathf.Round(GetValue(t));
}
