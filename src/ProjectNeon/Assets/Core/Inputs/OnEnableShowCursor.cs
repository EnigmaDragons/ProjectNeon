using UnityEngine;

public class OnEnableShowCursor : MonoBehaviour
{
    private void OnEnable()
    {
        CursorStateController.SetUnlocked();
    }
}
