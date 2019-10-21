using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public sealed class ClassTints : ScriptableObject
{
    [SerializeField] private Color defaultTint;
    [SerializeField] private ClassTint[] classTints;

    public Color TintFor(string className)
    {
        var matching = classTints.Where(x => x.ClassName.Equals(className)).ToArray();
        return matching.Length > 0 ? matching[0].Tint : defaultTint;
    }
}

[Serializable]
public sealed class ClassTint
{
    [SerializeField] private StringReference className;
    [SerializeField] private Color tint;

    public string ClassName => className.Value;
    public Color Tint => tint;
}

