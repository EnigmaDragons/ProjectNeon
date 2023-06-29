using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownNodeMap : MonoBehaviour
{
    [SerializeField] private int z;
    [SerializeField] private GameObject back;

    private bool _isEnabled;
    private DirectionalInputNodeMap _nodeMap;

    private void OnEnable()
    {
        if (!_isEnabled)
        {
            StartCoroutine(DelayedNodeMap());
            _isEnabled = true;
        }
    }
    
    private IEnumerator DelayedNodeMap()
    {
        yield return new WaitForSeconds(0.05f);
        if (_isEnabled)
        {
            var nodes = new List<DirectionalInputNode>();
            var previousNode = new DirectionalInputNode { Selectable = back };
            nodes.Add(previousNode);
            foreach (Transform child in transform)
            {
                if (child != transform && child.gameObject != back && child.gameObject.activeSelf)
                {
                    previousNode.Down = child.gameObject;
                    previousNode = new DirectionalInputNode { Selectable = child.gameObject, Up = previousNode.Selectable };
                    nodes.Add(previousNode);
                }
            }
            _nodeMap = new DirectionalInputNodeMap
            {
                Z = z,
                DefaultSelected = new []{nodes[1].Selectable},
                BackObject = back,
                Nodes = nodes.ToArray()
            };
            Message.Publish(new DirectionalInputNodeMapEnabled(_nodeMap));
        }
    }

    private void OnDisable()
    {
        _isEnabled = false;
        Message.Publish(new DirectionalInputNodeMapDisabled(_nodeMap));
    }
}