using UnityEngine;

public class PingPong : MonoBehaviour
{
    [SerializeField] private Vector3 pos1;
    [SerializeField] private Vector3 pos2;
    [SerializeField] private float duration = 3f;

    private bool _forwardDirection;
    private float _remaining;

    private void Awake() => transform.localPosition = pos1;
    
    private void FixedUpdate()
    {
        _remaining -= Time.deltaTime;
        var start = _forwardDirection ? pos1 : pos2;
        var dest = _forwardDirection ? pos2 : pos1;
        transform.localPosition = Vector3.Lerp(start, dest, (duration - _remaining) / duration);
        
        if (_remaining <= 0)
        {
            _forwardDirection = !_forwardDirection;
            _remaining = duration;
        }
    }
}
