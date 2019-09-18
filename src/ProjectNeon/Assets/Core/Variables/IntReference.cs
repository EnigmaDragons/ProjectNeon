using System;

[Serializable]
public class IntReference
{
    public bool UseConstant = true;
    public int ConstantValue;
    public IntVariable IntVariable;

    public int Value => UseConstant ? ConstantValue : IntVariable.Value;

    public IntReference() { }

    public IntReference(int value)
    {
        UseConstant = true;
        ConstantValue = value;
    }

    public static implicit operator int(IntReference reference)
    {
        return reference.Value;
    }
}
