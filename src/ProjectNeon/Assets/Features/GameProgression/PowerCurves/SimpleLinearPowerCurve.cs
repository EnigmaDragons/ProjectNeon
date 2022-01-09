using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/LinearPowerCurve")]
public class SimpleLinearPowerCurve : PowerCurve
{
    [SerializeField] private int start;
    [SerializeField] private int end;

    //usable by editor stuff only
    public int _start => start;
    public int _end => end;
    
    public override float GetValue(float t) => Mathf.Lerp(start, end, t);
}
