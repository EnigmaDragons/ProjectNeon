using UnityEngine;
using Vector2 = UnityEngine.Vector2;

[CreateAssetMenu(menuName = "Maps/Game Map 3")]
public class GameMap3 : ScriptableObject
{
    [SerializeField, UnityEngine.UI.Extensions.ReadOnly] public int id;
    [SerializeField] private MapPoints background;

    public GameObject Background => background.gameObject;
    public Vector2 StartingPoint => background.StartingPoint;
    public Vector2[] Points => background.AllPoints;
}