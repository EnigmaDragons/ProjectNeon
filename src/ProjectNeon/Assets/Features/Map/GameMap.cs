using UnityEngine;

[CreateAssetMenu]
public sealed class GameMap : ScriptableObject
{
    [SerializeField] private GameObject artPrototype;
    [SerializeField] private MapLocation2[] locations;
    [SerializeField] private int startingLocationIndex = 0;

    public GameObject ArtPrototype => artPrototype;
    public MapLocation2[] Locations => locations;
    public MapLocation2 StartingLocation => locations[startingLocationIndex];
}