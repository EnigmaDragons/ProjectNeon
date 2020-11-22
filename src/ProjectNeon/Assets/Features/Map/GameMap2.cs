using UnityEngine;

[CreateAssetMenu]
public class GameMap2 : ScriptableObject
{
    [SerializeField] private GameObject artPrototype;
    [SerializeField][Range(0,99)] private int minPaths;
    [SerializeField][Range(0,99)] private int maxPaths;

    public GameObject ArtPrototype => artPrototype;
    public int MinPaths => minPaths;
    public int MaxPaths => maxPaths;
}