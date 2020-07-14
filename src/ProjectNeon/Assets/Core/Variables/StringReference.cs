using System;

[Serializable]
public class StringReference
{
    public bool UseConstant = true;
    public string ConstantValue;
    public StringVariable Variable;

    public string Value => UseConstant ? ConstantValue : Variable.Value;

    public StringReference() : this(string.Empty) {}
    public StringReference(string value)
    {
        UseConstant = true;
        ConstantValue = value;
    }

    public static implicit operator string(StringReference reference) => reference.ToString();
    public override string ToString() => Value;
}
