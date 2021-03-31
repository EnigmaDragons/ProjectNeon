using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class LoopingRawImageUv : MonoBehaviour
{
    [SerializeField] private Vector2 speed = new Vector2(0.05f, 0.05f);
    [SerializeField] private bool useScaledTime = true;

    private RawImage _image;

    private void Awake() => _image = GetComponent<RawImage>();
    
    private void FixedUpdate()
    {
        var deltaTime = useScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
        var rectBefore = _image.uvRect;
        var positionVector = rectBefore.position + new Vector2(speed.x * deltaTime, speed.y * deltaTime);
        var sizeRect = rectBefore.size;
        _image.uvRect = new Rect(positionVector, sizeRect);
    }
}
