using UnityEngine;

public class ResetTimeScaleOnAwake : MonoBehaviour
{
    private void Awake() => Time.timeScale = 1;
}
