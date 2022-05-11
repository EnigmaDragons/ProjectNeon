using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapPoints : MonoBehaviour
{
    [SerializeField] private Transform startingPoint;
    [SerializeField] private List<Transform> points;

    public Vector2 StartingPoint => startingPoint.position;
    public Vector2[] AllPoints => points.Select(x => (Vector2) x.localPosition).ToArray();
}
