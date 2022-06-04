using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/SingleUseObjectDriftConfig")]
public class SingleUseObjectDriftConfig : ScriptableObject
{
    [SerializeField] private FloatReference driftDistance = new FloatReference(0.05f);
    [SerializeField] private FloatReference duration = new FloatReference(3.2f);
    [SerializeField] private FloatReference fadeDuration = new FloatReference(0.8f);

    public FloatReference DriftDistance => driftDistance;
    public FloatReference Duration => duration;
    public FloatReference FadeDuration => fadeDuration;

    public static float DefaultDuration = 3.2f;
    public static float DefaultDriftDistance = 0.05f;
    public static float DefaultFadeDuration = 0.8f;
}
