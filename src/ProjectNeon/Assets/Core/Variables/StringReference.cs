using System;

[Serializable]
public class StringReference
{
    public bool UseConstant = true;
    public string ConstantValue;
    [DTValidator.Optional] public StringVariable Variable;

    public string Value => UseConstant ? ConstantValue : Variable.Value;

    public StringReference() : this(string.Empty) {}
    public StringReference(string value)
    {
        UseConstant = true;
        ConstantValue = value;
    }

    public static implicit operator string(StringReference reference)
    {
        return reference.Value;
    }
}
