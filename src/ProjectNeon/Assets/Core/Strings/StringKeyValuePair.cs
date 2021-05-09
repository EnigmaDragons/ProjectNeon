using System;
using UnityEngine;

[Serializable]
public class StringKeyValuePair
{
    public string title;
    public StringReference Key;
    [SerializeField, TextArea(1, 12)]public string Value;
}
