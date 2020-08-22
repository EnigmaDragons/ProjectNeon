using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/PartyArea")]
public sealed class PartyArea : ScriptableObject
{
    [SerializeField] private Party party;
    [SerializeField] private Transform[] uiPositions;

    public Party Party => party;
    public Transform[] UiPositions => uiPositions;
    
    public PartyArea WithUiPositions(IEnumerable<Transform> positions)
    {
        uiPositions = positions.ToArray();
        return this;
    }
}
