using System;

[Serializable]
public class BoolReference
{
    public bool UseConstant = true;
    public bool ConstantValue;
    public BoolVariable Variable;

    public bool Value => UseConstant ? ConstantValue : Variable.Value;

    public BoolReference() : this(false) { }
    public BoolReference(bool value)
    {
        UseConstant = true;
        ConstantValue = value;
    }

    public static implicit operator bool(BoolReference reference)
    {
        return reference.Value;
    }
}
