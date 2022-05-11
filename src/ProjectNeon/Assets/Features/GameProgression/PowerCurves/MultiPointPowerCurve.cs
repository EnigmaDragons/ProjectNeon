using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/MultiPointPowerCurve")]
public class MultiPointPowerCurve : PowerCurve
{
    [SerializeField] private int[] points;
    
    //usable by editor stuff only
    public int _start => points[0];
    public int _end => points[points.Length - 1];

    public override float GetValue(float t)
    {
        //couldn't figure out how to make curves between the points
        for (int i = 0; i < points.Length - 1; i++)
        {
            var percentagePoint = (float)i / (points.Length - 1f);
            var nextPointPercentage = (float)(i + 1f) / (points.Length - 1f);
            if (t == percentagePoint)
                return points[i];
            if (t == nextPointPercentage)
                return points[i + 1];
            if (percentagePoint < t && nextPointPercentage > t)
                return Mathf.Lerp(points[i], points[i + 1], (t - percentagePoint) / (nextPointPercentage - percentagePoint));
        }
        return points[points.Length - 1];
    } 
}