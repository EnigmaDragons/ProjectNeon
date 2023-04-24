﻿using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EffectScopedData
{
    [SerializeField] private HashSet<string> trueConditions = new HashSet<string>();
    [SerializeField] private List<string> variableNames = new List<string>();
    [SerializeField] private List<float> variables = new List<float>();

    public void RecordTrueCondition(string name) => trueConditions.Add(name);
    public bool IsCondition(string name) => trueConditions.Contains(name);

    public void ResetScope()
    {
        trueConditions.Clear();
        variableNames.Clear();
        variables.Clear();
    }

    public void SetVariable(string name, float value) 
        => variables[GetIndex(name)] = value;

    public void AdjustVariable(string name, float value) 
        => variables[GetIndex(name)] += value;

    public float GetVariable(string name)
    {
        var index = variableNames.IndexOf(name);
        if (index == -1)
        {
            Log.Warn($"Missing named variable \"{name}\" when resolving formula");
            return 0;
        }

        return variables[index];
    }

    private int GetIndex(string name)
    {
        var index = variableNames.IndexOf(name);
        if (index == -1)
        {
            variableNames.Add(name);
            variables.Add(0);
            index = variableNames.Count - 1;
        }
        return index;
    }
}