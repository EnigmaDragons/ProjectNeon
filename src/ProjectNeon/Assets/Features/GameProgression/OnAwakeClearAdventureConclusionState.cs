using UnityEngine;

public class OnAwakeClearAdventureConclusionState : MonoBehaviour
{
    private void Awake() => AdventureConclusionState.Clear();
}
