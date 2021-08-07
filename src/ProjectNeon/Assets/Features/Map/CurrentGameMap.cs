using System;
using UnityEngine;

[Obsolete("MapView1")]
[CreateAssetMenu(menuName = "Maps/CurrentGameMap")]
public class CurrentGameMap : ScriptableObject
{
    [SerializeField] private GameMap map;

    public GameObject ArtPrototype => map.ArtPrototype;
    public MapLocation2[] Locations => map.Locations;
    public MapLocation2 StartingLocation => map.StartingLocation;

    public void SetMap(GameMap m) => map = m;
}