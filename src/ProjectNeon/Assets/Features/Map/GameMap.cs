using UnityEngine;

[CreateAssetMenu]
public sealed class GameMap : ScriptableObject
{
    [SerializeField] private GameObject artPrototype;
    [SerializeField] private MapLocation[] locations;
    [SerializeField] private int startingLocationIndex = 0;

    public GameObject ArtPrototype => artPrototype;
    public MapLocation[] Locations => locations;
    public MapLocation StartingLocation => locations[startingLocationIndex];
}