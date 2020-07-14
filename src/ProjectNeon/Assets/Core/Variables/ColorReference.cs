using System;
using UnityEngine;

[Serializable]
public class ColorReference
{
    public bool UseConstant = true;
    public Color ConstantValue;
    public ColorVariable Variable;

    public Color Value => UseConstant ? ConstantValue : Variable.Value;

    public ColorReference() : this(Color.magenta) { }
    public ColorReference(Color value)
    {
        UseConstant = true;
        ConstantValue = value;
    }

    public static implicit operator Color(ColorReference reference)
    {
        return reference.Value;
    }
}
