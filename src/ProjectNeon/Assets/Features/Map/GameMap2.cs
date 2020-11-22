using UnityEngine;

[CreateAssetMenu]
public class GameMap2 : ScriptableObject
{
    [SerializeField] private GameObject artPrototype;
    [SerializeField][Range(0,99)] private int minPaths;
    [SerializeField][Range(0,99)] private int maxPaths;
    [SerializeField] private float bottomMargin;
    [SerializeField] private float leftMargin;
    [SerializeField] private float rightMargin;
    [SerializeField] private float topMargin = 254;
    
    public GameObject ArtPrototype => artPrototype;
    public int MinPaths => minPaths;
    public int MaxPaths => maxPaths;
    public float BottomMargin => bottomMargin;
    public float LeftMargin => leftMargin;
    public float RightMargin => rightMargin;
    public float TopMargin => topMargin;
}