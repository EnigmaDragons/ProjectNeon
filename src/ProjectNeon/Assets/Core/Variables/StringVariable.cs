﻿using System;
using UnityEngine;

[CreateAssetMenu(menuName = "String Variable", fileName = "NewStringVariable", order = -20)]
public class StringVariable : ScriptableObject, ILocalizeTerms
{
    [SerializeField]
    private string value = string.Empty;

    public string Value
    {
        get { return value; }
        set { this.value = value; }
    }

    public void SetValue(string str) => value = str;

    public override bool Equals(object other)
    {
        if (other is string)
            return value.Equals(other);
        if (other is StringVariable v)
            return value.Equals(v.value);
        if (other is StringReference r)
            return value.Equals(r.Value);
        return false;
    }

    public string[] GetLocalizeTerms()
    {
        if (name.ContainsAnyCase("Archetype"))
            return new[] { $"Archetypes/{Value}" };
        
        return Array.Empty<string>();
    }
}
