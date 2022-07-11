using System;

[Serializable]
public class ShowNamedTarget
{
    public StringReference TargetName;
    
    public ShowNamedTarget() 
        : this(string.Empty) {}

    public ShowNamedTarget(string name)
    {
        TargetName = new StringReference(name);
    }
}
