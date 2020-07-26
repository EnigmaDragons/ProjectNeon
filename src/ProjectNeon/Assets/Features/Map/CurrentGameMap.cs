using UnityEngine;

[CreateAssetMenu]
public class CurrentGameMap : ScriptableObject
{
    [SerializeField] private GameMap map;
    
    public MapLocation[] Locations => map.Locations;
    public MapLocation StartingLocation => map.StartingLocation;
}