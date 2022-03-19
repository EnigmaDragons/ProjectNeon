using System;
using UnityEngine;

[Serializable]
public class CorpTypedNode
{
    [SerializeField] private StaticCorp corp;
    [SerializeField] private MapNodeGameObject3 nodeGameObject;

    public Corp Corp => corp;
    public MapNodeGameObject3 Object => nodeGameObject;
}
