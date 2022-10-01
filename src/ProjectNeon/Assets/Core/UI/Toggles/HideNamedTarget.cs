using System;

[Serializable]
public class HideNamedTarget
{
    public StringReference TargetName;
    
    public HideNamedTarget() 
        : this(string.Empty) {}

    public HideNamedTarget(string name)
    {
        TargetName = new StringReference(name);
    }
}
