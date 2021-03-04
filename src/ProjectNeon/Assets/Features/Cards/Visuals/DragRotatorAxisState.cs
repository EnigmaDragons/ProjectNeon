using System;

[Serializable]
public class DragRotatorAxisState
{
    public float m_ForceMultiplier = 4;
    public float m_MinDegrees = -20;
    public float m_MaxDegrees = 20;
    public float m_RestSeconds = 1.2f;
}