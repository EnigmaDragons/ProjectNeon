using System;
using UnityEngine;

public class EnableStateObservable : MonoBehaviour
{
    private Action _onChange = () => { };

    public void Watch(Action onChange) => _onChange = onChange;

    private void OnEnable() => _onChange();
    private void OnDisable() => _onChange();
}