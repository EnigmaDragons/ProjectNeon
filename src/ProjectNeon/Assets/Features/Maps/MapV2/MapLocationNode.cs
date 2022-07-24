using System;
using TMPro;
using UnityEngine;

[Obsolete("MapView1")]
public sealed class MapLocationNode : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameLabel;

    private CurrentGameMap _map;
    private MapLocation2 _location;
    
    public void Init(CurrentGameMap map, MapLocation2 l)
    {
        _map = map;
        _location = l;
        nameLabel.text = l.DisplayName;
    }
}