using UnityEngine;

[CreateAssetMenu]
public class GameMap2 : ScriptableObject
{
    [SerializeField] private GameObject artPrototype;
    [SerializeField][Range(0,99)] private int pathLength;
    [SerializeField][Range(0,99)] private int minPaths;
    [SerializeField][Range(0,99)] private int maxPaths;
    [SerializeField] private AnimationCurve powerCurve;

    public GameObject ArtPrototype => artPrototype;
    public int PathLength => pathLength;
    public int MinPaths => minPaths;
    public int MaxPaths => maxPaths;
    public int GetPowerLevel(float percent) => (int)Mathf.Round(powerCurve.Evaluate(percent));
}