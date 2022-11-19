using System;
using UnityEngine;

[Serializable]
public class StringKeyTermPair
{
    public string title;
    public StringReference Key;
    public StringReference Term;
    [SerializeField, TextArea(1, 12)]public string Value;
}