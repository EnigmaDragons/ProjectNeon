using UnityEngine;

[CreateAssetMenu]
public sealed class GameMap : ScriptableObject
{
    [SerializeField] private MapLocation[] locations;
    [SerializeField] private int startingLocationIndex = 0;
    
    public MapLocation[] Locations => locations;
    public MapLocation StartingLocation => locations[startingLocationIndex];
}