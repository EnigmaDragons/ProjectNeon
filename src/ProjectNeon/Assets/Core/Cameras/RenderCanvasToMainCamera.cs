using UnityEngine;

[RequireComponent(typeof(Canvas))]
public sealed class RenderCanvasToMainCamera : MonoBehaviour
{
    void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }
}
