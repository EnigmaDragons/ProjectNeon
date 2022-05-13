using System;
using UnityEngine;

[Serializable]
public sealed class HeroFlavorDetails
{
    public string RoleDescription;
    [SerializeField, TextArea(2, 2)] public string HeroDescription;
    [SerializeField, TextArea(4, 10)] public string BackStory;
}
