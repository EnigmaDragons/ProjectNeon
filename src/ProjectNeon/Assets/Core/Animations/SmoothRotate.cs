using UnityEngine;

public class SmoothRotate : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private Quaternion initial;
    [SerializeField] private Quaternion final;
    [SerializeField] private float duration;

    private bool _isFinished = true;
    private float _elapsed;

    public void InitInitialPosition() => target.transform.rotation = initial;
    
    public void Begin()
    {
        _isFinished = false;
        _elapsed = 0;
        InitInitialPosition();
    }

    private void FixedUpdate()
    {
        if (_isFinished)
            return;
        _elapsed += Time.deltaTime;
        target.transform.rotation = Quaternion.Lerp(initial, final, Mathf.Clamp(_elapsed /duration, 0, 1));
        if (_elapsed >= duration)
            _isFinished = true;
    }
}