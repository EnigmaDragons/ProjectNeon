using UnityEngine;

public class InitErrorHandler : MonoBehaviour
{
    private void Awake() => ErrorHandler.Init();

}
