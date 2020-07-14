using System;
using UnityEngine;

[Serializable]
public class Vector3Reference
{
    public bool UseConstant = true;
    public Vector3 ConstantValue;
    public Vector3Variable Variable;

    public Vector3 Value => UseConstant ? ConstantValue : Variable.Value;

    public Vector3Reference() : this(Vector3.zero) { }
    public Vector3Reference(Vector3 value)
    {
        UseConstant = true;
        ConstantValue = value;
    }

    public static implicit operator Vector3(Vector3Reference reference)
    {
        return reference.Value;
    }
}
