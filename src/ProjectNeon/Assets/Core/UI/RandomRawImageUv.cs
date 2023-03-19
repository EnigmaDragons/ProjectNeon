using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class RandomRawImageUv : MonoBehaviour
{
    [SerializeField] private int updatesPerSeconds = 6;
    
    private RawImage _image;
    private float _cooldownMs;
    private static readonly DeterministicRng RngInstance = DeterministicRng.CreateRandom();

    private void Awake() => _image = GetComponent<RawImage>();
    
    private void FixedUpdate()
    {
        _cooldownMs -= Time.unscaledDeltaTime;
        if (_cooldownMs > 0)
            return;
        
        var rectBefore = _image.uvRect;
        var positionVector = new Vector2(RngInstance.Float(), RngInstance.Float());
        var sizeRect = rectBefore.size;
        _image.uvRect = new Rect(positionVector, sizeRect);
        
        if (_cooldownMs <= 0)
            _cooldownMs = (1f / updatesPerSeconds);
    }
}
