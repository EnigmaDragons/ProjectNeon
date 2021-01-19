using System;
using UnityEngine;

[Serializable]
public class StringKeyValuePair
{
    public string Name => Key.Value;
    public StringReference Key;
    [SerializeField, TextArea(1, 12)]public string Value;
}
