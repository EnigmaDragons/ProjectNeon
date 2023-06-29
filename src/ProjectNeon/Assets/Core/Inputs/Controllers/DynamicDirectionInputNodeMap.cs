﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DynamicDirectionInputNodeMap : MonoBehaviour
{
    private const float MinimumPercentage = 0.5f;
    private const float MaximumDifference = 150;
    
    [SerializeField] private int z;
    [SerializeField] private GameObject backObject;
    [SerializeField] private RectTransform[] nodes;
    [SerializeField] private SelectableComponent[] dynamicComponents; 
    [SerializeField] private SelectablesContainer[] containers;
    [SerializeField] private SelectablesContainer defaultSelectedContainer;
    [SerializeField] private SelectableComponent defaultSelectedComponent;
    [SerializeField] private RectTransform defaultSelectedNode;

    private DirectionalInputNodeMap _nodeMap;

    private void Awake()
    {
        foreach (var component in dynamicComponents)
            component.Observe(Refresh);
        foreach (var container in containers)
            container.Observe(Refresh);
    }
    
    private void OnEnable()
    {
        RefreshNodeMap();
        Message.Publish(new DirectionalInputNodeMapEnabled(_nodeMap));
    }

    private void Refresh()
    {
        if (gameObject.activeInHierarchy)
        {
            var outdatedMap = _nodeMap;
            RefreshNodeMap();
            Message.Publish(new DirectionalInputNodeMapChanged(outdatedMap, _nodeMap));
        }
    }

    private void OnDisable()
        => Message.Publish(new DirectionalInputNodeMapDisabled(_nodeMap));

    private void RefreshNodeMap()
    {
        var allObjects = nodes.Where(x => x.gameObject.activeInHierarchy).Concat(dynamicComponents.Where(x => x.gameObject.activeInHierarchy).Select(x => x.Rect).Concat(containers.SelectMany(x => x.GetSelectables()))).ToArray();
        var result = new List<DirectionalInputNode>();
        foreach (var obj in allObjects)
        {
            var node = new DirectionalInputNode { Selectable = obj.gameObject };
            var position = obj.position;
            var size = obj.sizeDelta;
            node.Up = allObjects
                .Where(x => x.position.y > position.y && Math.Abs(x.position.y - position.y) > Math.Abs(x.position.x - position.x))
                .OrderBy(x => x.position.y)
                .ThenBy(x => Math.Abs(x.position.x - position.x))
                .FirstOrDefault()?.gameObject;
            if (node.Up == null)
                node.Up = allObjects.Where(x => x.position.y > position.y + Math.Min(MaximumDifference, MinimumPercentage * size.y))
                    .OrderBy(x => x.position.y)
                    .ThenBy(x => Math.Abs(x.position.x - position.x))
                    .FirstOrDefault()?.gameObject;
            node.Down = allObjects
                .Where(x => x.position.y < position.y && Math.Abs(x.position.y - position.y) > Math.Abs(x.position.x - position.x))
                .OrderByDescending(x => x.position.y)
                .ThenBy(x => Math.Abs(x.position.x - position.x))
                .FirstOrDefault()?.gameObject;
            if (node.Down == null)
                node.Down = allObjects.Where(x => x.position.y < position.y - Math.Min(MaximumDifference, MinimumPercentage * size.y))
                    .OrderByDescending(x => x.position.y)
                    .ThenBy(x => Math.Abs(x.position.x - position.x))
                    .FirstOrDefault()?.gameObject;
            node.Left = allObjects
                .Where(x => x.position.x < position.x && Math.Abs(x.position.x - position.x) > Math.Abs(x.position.y - position.y))
                .OrderByDescending(x => x.position.x)
                .ThenBy(x => Math.Abs(x.position.y - position.y))
                .FirstOrDefault()?.gameObject;
            if (node.Left == null)
                node.Left = allObjects.Where(x => x.position.x < position.x - Math.Min(MaximumDifference, MinimumPercentage * size.x))
                    .OrderBy(x => x.position.x)
                    .ThenBy(x => Math.Abs(x.position.y - position.y))
                    .FirstOrDefault()?.gameObject;
            node.Right = allObjects
                .Where(x => x.position.x > position.x && Math.Abs(x.position.x - position.x) > Math.Abs(x.position.y - position.y))
                .OrderBy(x => x.position.x)
                .ThenBy(x => Math.Abs(x.position.y - position.y))
                .FirstOrDefault()?.gameObject;
            if (node.Right == null)
                node.Right = allObjects.Where(x => x.position.x > position.x + Math.Min(MaximumDifference, MinimumPercentage * size.x))
                    .OrderBy(x => x.position.x)
                    .ThenBy(x => Math.Abs(x.position.y - position.y))
                    .FirstOrDefault()?.gameObject;
            result.Add(node);
        }
        var defaultSelected = new List<GameObject>();
        if (defaultSelectedContainer != null && defaultSelectedContainer.GetDefault() != null)
            defaultSelected.Add(defaultSelectedContainer.GetDefault().gameObject);
        if (defaultSelectedComponent != null)
            defaultSelected.Add(defaultSelectedComponent.gameObject);
        if (defaultSelectedNode != null)
            defaultSelected.Add(defaultSelectedNode.gameObject);
        _nodeMap = new DirectionalInputNodeMap
        {
            Z = z,
            BackObject = backObject,
            DefaultSelected = defaultSelected.ToArray(),
            Nodes = result.ToArray()
        };
    }
}