using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class StoryResolution
{
    public float Chance;
    public StoryResult Result;
    [FormerlySerializedAs("SuccessText")] [TextArea(2, 7)] public string StoryText;
}
