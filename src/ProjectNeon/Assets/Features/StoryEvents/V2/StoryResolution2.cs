using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class StoryResolution2
{
    public float Chance;
    [Header("A. Next Story Event")] public StoryEvent2 ContinueWith;
    [Header("B. Final Resolution")]
    public StoryResult Result;

    public int ResultNumber;
    [FormerlySerializedAs("SuccessText")] [TextArea(2, 7)] public string StoryText;

    public bool HasContinuation => ContinueWith != null;
    public int EstimatedCreditsValue => Result.EstimatedCreditsValue;
}