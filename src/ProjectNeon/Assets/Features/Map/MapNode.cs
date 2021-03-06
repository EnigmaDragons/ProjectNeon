﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class MapNode
{
    public string NodeId;
    public MapNodeType Type;
    public List<string> ChildrenIds;
    public int X;
    public int Y;

    public static MapNode GenerateNew(int x, int y)
        => new MapNode { NodeId = Guid.NewGuid().ToString(), Type = MapNodeType.Unknown, X = x, Y = y, ChildrenIds = new List<string>() };
}