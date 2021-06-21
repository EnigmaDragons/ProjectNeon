using UnityEngine;
using Vector2 = UnityEngine.Vector2;

[CreateAssetMenu(menuName = "Maps/Game Map 3")]
public class GameMap3 : ScriptableObject
{
    [SerializeField, UnityEngine.UI.Extensions.ReadOnly] public int id;
    [SerializeField] private GameObject background;
    [SerializeField] private int startingPoint;
    [SerializeField] private Vector2[] points;

    public GameObject Background => background;
    public Vector2 StartingPoint => points[startingPoint];
    public Vector2[] Points => points;
}