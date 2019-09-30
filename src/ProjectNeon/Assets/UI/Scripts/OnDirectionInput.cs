using System;
using UnityEngine;
using UnityEngine.Events;

public class OnDirectionInput : MonoBehaviour
{
    [SerializeField] private GameEvent activateOn;
    [SerializeField] private GameEvent deactivateOn;
    [SerializeField] private UnityEvent onLeft;
    [SerializeField] private UnityEvent onRight;
    [SerializeField] private UnityEvent onUp;
    [SerializeField] private UnityEvent onDown;

    private bool _isActive;
    private float _lastHDir = 0;
    private float _lastVDir = 0;
    
    private void OnEnable()
    {
        activateOn.Subscribe(() => _isActive = true, this);
        deactivateOn.Subscribe(() => _isActive = false, this);
    }

    private void OnDisable()
    {
        activateOn.Unsubscribe(this);
        deactivateOn.Unsubscribe(this);
    }

    void Update()
    {
        if (!_isActive)
            return;

        var hDir = Input.GetAxisRaw("Horizontal");
        if (Math.Abs(hDir - _lastHDir) > 0.1f)
        {
            _lastHDir = hDir;
            if (hDir < 0)
                onLeft.Invoke();
            if (hDir > 0)
                onRight.Invoke();
        }

        var vDir = Input.GetAxisRaw("Vertical");
        if (Math.Abs(vDir - _lastVDir) > 0.1f)
        {
            _lastVDir = vDir;
            if (vDir < 0)
                onDown.Invoke();
            if (vDir > 0)
                onUp.Invoke();
        }
    }
}
