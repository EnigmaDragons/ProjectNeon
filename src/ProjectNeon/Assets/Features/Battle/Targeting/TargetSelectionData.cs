using System;
using UnityEngine;

[Serializable]
public class TargetSelectionData
{
    [SerializeField] private Scope scope;
    [SerializeField] private Group group;

    public Scope Scope => scope;
    public Group Group => group;
}