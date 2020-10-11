using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/PartyArea")]
public sealed class PartyArea : ScriptableObject
{
    [SerializeField] private Party party;
    [SerializeField] private Transform[] uiPositions;
    [SerializeField] private Vector3[] centerPoints;

    public Party Party => party;
    public Transform[] UiPositions => uiPositions;
    public Vector3[] CenterPositions => centerPoints;
    
    public PartyArea WithUiPositions(IEnumerable<Transform> positions, IEnumerable<Vector3> centers)
    {
        uiPositions = positions.ToArray();
        centerPoints = centers.ToArray();
        return this;
    }
}
