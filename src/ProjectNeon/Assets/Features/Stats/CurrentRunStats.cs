using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/CurrentRunStats")]
public class CurrentRunStats : ScriptableObject
{
    [SerializeField] private float totalElapsed;

    public float TotalElapsed
    {
        get => totalElapsed;
        set => totalElapsed = value;
    }
    
    public void Init(RunStats stats)
    {
        totalElapsed = stats.TimeElapsedSeconds * 1000;
    }

    public RunStats GetData()
    {
        return new RunStats
        {
            TimeElapsedSeconds = (totalElapsed / 1000f).CeilingInt(),
        };
    }
}
