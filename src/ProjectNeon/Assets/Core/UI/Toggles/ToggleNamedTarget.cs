using System;

[Serializable]
public class ToggleNamedTarget
{
    public StringReference TargetName;
    
    public ToggleNamedTarget() 
        : this(string.Empty) {}

    public ToggleNamedTarget(string name)
    {
        TargetName = new StringReference(name);
    }
}