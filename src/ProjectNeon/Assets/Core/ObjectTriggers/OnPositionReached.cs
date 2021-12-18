using System;
using UnityEngine;

public class OnPositionReached : MonoBehaviour
{
    [SerializeField] private Vector3 dimensions;
    [SerializeField] private Vector3 boundaries;

    private bool _triggered;
    private Action _action = NoOp;

    public void SetAction(Action a) => _action = a;

    private static void NoOp(){}
    
    private void Update()
    {
        if (_triggered)
            return;
        
        var pos = transform.position;
        if (dimensions.x < 0 && pos.x <= boundaries.x)
            Trigger();
        if (dimensions.x > 0 && pos.x >= boundaries.x)
            Trigger();
        if (dimensions.y < 0 && pos.y <= boundaries.y)
            Trigger();
        if (dimensions.y > 0 && pos.y >= boundaries.y)
            Trigger();
        if (dimensions.z < 0 && pos.z <= boundaries.z)
            Trigger();
        if (dimensions.z > 0 && pos.z >= boundaries.z)
            Trigger();
    }

    private void Trigger()
    {
        _triggered = true;
        _action();
    }
}