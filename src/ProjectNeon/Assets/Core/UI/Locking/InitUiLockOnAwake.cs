using UnityEngine;

public class InitUiLockOnAwake : MonoBehaviour
{
    private void OnAwake() => UiLock.Init();
}
