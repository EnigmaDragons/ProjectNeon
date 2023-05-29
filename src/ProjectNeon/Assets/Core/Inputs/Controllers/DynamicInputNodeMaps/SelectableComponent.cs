using System;
using UnityEngine;

public class SelectableComponent : MonoBehaviour
{
    private Action _onChange;

    public RectTransform Rect;

    private void Awake() => Rect = GetComponent<RectTransform>();
    
    public void Observe(Action onChange) => _onChange = onChange;

    private void OnEnable()
    {
        if (_onChange != null)
            _onChange();
    }

    private void OnDisable()
    {
        if (_onChange != null)
            _onChange();
    }
}