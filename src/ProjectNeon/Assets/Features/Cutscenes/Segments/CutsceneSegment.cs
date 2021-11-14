using UnityEngine;

public abstract class CutsceneSegment : ScriptableObject
{
    public abstract void Start();
    public abstract void FinishInstantly();
}
