using System;
using UnityEngine;

[Serializable]
public sealed class StatusIconDefinition
{
    [SerializeField] public string Name;
    [PreviewSprite] [SerializeField] public Sprite Icon;
}
