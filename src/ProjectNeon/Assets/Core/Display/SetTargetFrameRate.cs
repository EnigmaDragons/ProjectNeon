using UnityEngine;

public sealed class SetTargetFrameRate : MonoBehaviour
{
    [SerializeField] private int framesPerSecond = 60;

    void Awake() => Application.targetFrameRate = framesPerSecond;
}
