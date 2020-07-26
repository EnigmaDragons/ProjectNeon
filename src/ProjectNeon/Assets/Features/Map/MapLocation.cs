using UnityEngine;

[CreateAssetMenu]
public sealed class MapLocation : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField] private string description;
    [SerializeField] private Vector2 geoPosition;

    public string DisplayName => displayName;
    public string Description => description;
    public Vector2 GeoPosition => geoPosition;
}