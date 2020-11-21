using System;
using UnityEngine;

[Serializable]
public class StoryResolution
{
    public float Chance;
    public StoryResult Result;
    [TextArea(2, 7)] public string SuccessText;
    [TextArea(2, 7)] public string FailureText;
}
