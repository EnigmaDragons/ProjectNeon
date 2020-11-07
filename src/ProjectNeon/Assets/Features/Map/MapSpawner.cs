
using UnityEngine;

public sealed class MapSpawner : MonoBehaviour
{
    [SerializeField] private CurrentGameMap map;
    [SerializeField] private MapLocationNode nodePrototype;
    [SerializeField] private GameObject tokenPrototype;
    [SerializeField] private AdventureProgress progress;

    private void Awake()
    {
        Instantiate(map.ArtPrototype, transform);
        map.Locations.ForEach(SpawnNode);
        SpawnToken();
    }

    private void SpawnNode(MapLocation2 l)
    {
        var o = Instantiate(nodePrototype, transform);
        var rectTransform = o.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = l.GeoPosition;
        o.Init(map, l);
    }

    private void SpawnToken()
    {
        progress.InitIfNeeded();
        var o = Instantiate(tokenPrototype, transform);
        var rectTransform = o.GetComponent<RectTransform>();
        rectTransform.anchoredPosition += map.Locations[progress.CurrentStageSegmentIndex].GeoPosition;
        var floating = o.GetComponent<Floating>();
        if (floating != null)
            floating.enabled = true;
    }
}