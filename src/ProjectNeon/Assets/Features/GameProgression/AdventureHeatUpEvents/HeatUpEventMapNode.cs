using System;

[Serializable]
public class HeatUpEventMapNode
{
    public MapNodeType NodeType;
    public StaticGlobalEffect UnvisitedGlobalEffect;
    public StaticGlobalEffect VisitedGlobalEffect;
}
