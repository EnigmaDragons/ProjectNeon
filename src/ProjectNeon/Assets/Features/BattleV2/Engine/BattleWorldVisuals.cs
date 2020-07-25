
using System.Collections;
using UnityEngine;

public class BattleWorldVisuals : MonoBehaviour
{
    [SerializeField] private PartyVisualizerV2 party;

    public IEnumerator Setup()
    {
        yield return party.Setup();
    }
}
