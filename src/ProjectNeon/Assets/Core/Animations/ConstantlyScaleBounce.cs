using UnityEngine;

public class ConstantlyScaleBounce : MonoBehaviour
{
    [SerializeField] private Vector3 scaleAdjust = new Vector3(1, 1, 1);
    [SerializeField] private float speedFactor = 1f;
    [SerializeField] private bool randomizeStart = true;

    private float _offset = 0f;
    private Vector3 _minScale;
    private Vector3 _maxScale;

    private void Awake()
    {
        _minScale = transform.localScale - scaleAdjust;
        _maxScale = transform.localScale + scaleAdjust;
        if (randomizeStart)
            _offset = Rng.Float();
    }

    private void FixedUpdate()
    {
        var p = Mathf.PingPong(Time.timeSinceLevelLoad * speedFactor + _offset, 1);
        transform.localScale = Vector3.Lerp(_minScale, _maxScale, p);
    }
}
