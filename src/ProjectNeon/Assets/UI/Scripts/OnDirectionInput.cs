using System;
using UnityEngine;
using UnityEngine.Events;

public class OnDirectionInput : MonoBehaviour
{
    [SerializeField] private UnityEvent onLeft;
    [SerializeField] private UnityEvent onRight;
    [SerializeField] private UnityEvent onUp;
    [SerializeField] private UnityEvent onDown;

    private float _lastHDir = 0;
    private float _lastVDir = 0;

    void Update()
    {
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
