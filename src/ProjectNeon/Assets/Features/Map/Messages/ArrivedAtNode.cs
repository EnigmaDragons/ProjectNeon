using UnityEngine;

public class ArrivedAtNode
{
    public Transform UiSource { get; }
    public MapNodeType NodeType { get; }

    public ArrivedAtNode(Transform uiSource, MapNodeType nodeType)
    {
        UiSource = uiSource;
        NodeType = nodeType;
    }
}
