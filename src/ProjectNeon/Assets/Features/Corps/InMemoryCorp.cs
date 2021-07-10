using UnityEngine;

public class InMemoryCorp : Corp
{
    public string Name { get; set; } = "None";
    public Sprite Logo { get; }
    public Color Color { get; } = Color.white;
    public string[] RivalCorpNames { get; set; } = new string[0];

    public static implicit operator string(InMemoryCorp c) => c.Name;
}