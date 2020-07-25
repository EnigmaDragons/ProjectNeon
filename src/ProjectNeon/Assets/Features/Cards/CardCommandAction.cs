using System;
using UnityEngine;

[Serializable]
public class CardCommandAction
{
    [SerializeField] private CardBattleActionType type;
    [SerializeField] private TargetSelectionData targetSelectionData;
}