using UnityEngine;

public class EnemyUiPoint : MonoBehaviour
{
    private Camera _camera;
    private Transform _target;
    
    private void Awake() => _camera = Camera.main;

    public void Bind(Transform uiTarget) => _target = uiTarget;
    
    private void Update()
    {
        if (_target == null)
            return;
        
        var newPos = _camera.WorldToScreenPoint(transform.position);
        _target.position = newPos;
    }
}
