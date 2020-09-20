using UnityEngine;

[CreateAssetMenu]
public class CurrentGameMap : ScriptableObject
{
    [SerializeField] private GameMap map;

    public GameObject ArtPrototype => map.ArtPrototype;
    public MapLocation[] Locations => map.Locations;
    public MapLocation StartingLocation => map.StartingLocation;

    public void SetMap(GameMap m) => map = m;
}