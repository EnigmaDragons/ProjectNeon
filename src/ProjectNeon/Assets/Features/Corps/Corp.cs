using UnityEngine;

public interface Corp
{
    string Name { get; }
    Sprite Logo { get; }
    Color Color { get; }
    string[] RivalCorpNames { get; }
}
