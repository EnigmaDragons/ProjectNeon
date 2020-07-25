using System;
using UnityEngine;

[Serializable]
public class CardCommandAction
{
    [SerializeField] private CardCommandActionType type;
    [SerializeField] private TargetSelectionData targetSelectionData;

    public TargetSelectionData TargetSelectionData => targetSelectionData;
}