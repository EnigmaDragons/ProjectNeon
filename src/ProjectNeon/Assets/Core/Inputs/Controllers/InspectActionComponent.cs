using System;
using UnityEngine;

public class InspectActionComponent : MonoBehaviour
{
    private Action _action;

    public void Bind(Action action) => _action = action;

    public void Execute() => _action();
}