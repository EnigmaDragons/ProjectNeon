using UnityEngine;

public class ResetMouseDragStateOnEnable : MonoBehaviour
{
    private void OnEnable() => MouseDragState.Set(false);
}
