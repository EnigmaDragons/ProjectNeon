using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/LinearPowerCurve")]
public class SimpleLinearPowerCurve : PowerCurve
{
    [SerializeField] private int start;
    [SerializeField] private int end;

    public override float GetValue(float t) => Mathf.Lerp(start, end, t);
}
