using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class DirectionalInputNodeMap
{
    public int Z;
    public GameObject[] DefaultSelected;
    public GameObject BackObject;
    public GameObject NextObject;
    public GameObject PreviousObject;
    public GameObject NextObject2;
    public GameObject PreviousObject2;
    public GameObject InspectObject;
    public DirectionalInputNode[] Nodes;

    public DirectionalInputNode DefaultSelectedNode 
        => GetNode(DefaultSelected.FirstOrDefault(x => x.activeInHierarchy));

    public DirectionalInputNode GetNodeInDirection(DirectionalInputNode node, InputDirection direction)
    {
        var hashSet = new HashSet<GameObject>();
        while (!hashSet.Contains(node.Selectable))
        {
            hashSet.Add(node.Selectable);
            var obj = node.GetObjectInDirection(direction);
            if (obj == null)
                return null;
            node = GetNode(obj);
            if (node?.Selectable == null)
                return null;
            if (node.Selectable.activeInHierarchy)
                return node;
        }
        return null;
    }
    
    public DirectionalInputNode GetNode(GameObject obj) 
        => obj == null ? null : Nodes.SingleOrDefault(x => x.Selectable == obj);
}