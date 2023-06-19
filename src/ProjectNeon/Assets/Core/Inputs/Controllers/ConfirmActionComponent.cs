using System;
using UnityEngine;

public class ConfirmActionComponent : MonoBehaviour
{
    private Action _action;

    public void Bind(Action action) => _action = action;

    public void Execute() => _action();
}