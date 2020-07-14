using UnityEngine;

public sealed class ColorVariable : ScriptableObject
{
    [SerializeField] private Color value;

    public Color Value
    {
        get => value;
        set => this.value = value;
    }

    public ColorVariable(Color color) => value = color;
}
