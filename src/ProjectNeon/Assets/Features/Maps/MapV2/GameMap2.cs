using UnityEngine;

[CreateAssetMenu(menuName = "Maps/GameMap2")]
public class GameMap2 : ScriptableObject
{
    [SerializeField] private MovableMap artPrototype;
    [SerializeField][Range(0,99)] private int minPaths;
    [SerializeField][Range(0,99)] private int maxPaths;
    
    public MovableMap ArtPrototype => artPrototype;
    public int MinPaths => minPaths;
    public int MaxPaths => maxPaths;
}