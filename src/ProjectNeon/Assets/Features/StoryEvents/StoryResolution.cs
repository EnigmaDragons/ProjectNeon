using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class StoryResolution
{
    public float Chance;
    [Header("A. Next Story Event")] public StoryEvent ContinueWith;
    [Header("B. Final Resolution")]
    public StoryResult Result;
    [FormerlySerializedAs("SuccessText")] [TextArea(2, 7)] public string StoryText;

    public bool HasContinuation => ContinueWith != null;
    public int EstimatedCreditsValue => HasContinuation ? 0 : Result.EstimatedCreditsValue;
}
