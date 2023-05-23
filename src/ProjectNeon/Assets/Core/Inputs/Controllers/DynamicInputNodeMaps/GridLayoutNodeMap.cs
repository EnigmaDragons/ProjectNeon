using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GridLayoutNodeMap : MonoBehaviour
{
    [SerializeField] private int z;
    [SerializeField] private GridLayoutGroup group;
    [SerializeField] private GameObject back;
    [SerializeField] private GridLayoutAdditionalNode[] AdditionalNodes;

    private Dictionary<GridOuterPosition, GameObject> _additionalNodeMap;
    private DirectionalInputNodeMap _nodeMap;

    private void Awake()
    {
        _additionalNodeMap = AdditionalNodes.ToDictionary(x => x.Position, x => x.Selectable);
    }
    
    private void OnEnable()
    {
        var grid = new List<List<GameObject>>();
        foreach (Transform child in group.transform)
        {
            if (child.transform == group.transform || !child.gameObject.activeInHierarchy)
                continue;

            if (!grid.Any())
            {
                grid.Add(new List<GameObject> { child.gameObject });
                continue;
            }

            var row = grid.FirstIndexOf(x => Math.Abs(x[0].transform.localPosition.y - child.localPosition.y) < 0.1f);
            if (row == -1)
            {
                row = grid.FirstIndexOf(x => child.localPosition.y > x[0].transform.localPosition.y);
                if (row == -1)
                    grid.Add(new List<GameObject> {child.gameObject});
                else
                    grid.Insert(row, new List<GameObject> { child.gameObject });
                continue;
            }

            var column = grid[row].FirstIndexOf(x => child.localPosition.x < x.transform.localPosition.x);
            if (column == -1)
                grid[row].Add(child.gameObject);
            else
                grid[row].Insert(column, child.gameObject);
        }

        if (!grid.Any())
            return;
        for (var row = 0; row < grid.Count; row++)
            for (var column = 0; column < grid[row].Count; column++)
                grid[row][column] = grid[row][column].GetComponentInChildren<SelectableComponent>().gameObject;
        
        var nodes = new List<DirectionalInputNode>();
        var lastRow = grid.Count - 1;
        for (var row = 0; row <= lastRow; row++)
        {
            var lastColumn = grid[row].Count - 1;
            for (var column = 0; column <= lastColumn; column++)
            {
                var node = new DirectionalInputNode { Selectable = grid[row][column] };
                
                if (row == 0)
                {
                    if (column == 0 && _additionalNodeMap.ContainsKey(GridOuterPosition.TopLeft))
                        node.Up = _additionalNodeMap[GridOuterPosition.TopLeft];
                    else if (column == lastColumn && _additionalNodeMap.ContainsKey(GridOuterPosition.TopRight))
                        node.Up = _additionalNodeMap[GridOuterPosition.TopRight];
                    else 
                        node.Up = GetFirst(GridOuterPosition.TopCenter, GridOuterPosition.TopLeft, GridOuterPosition.TopRight);
                }
                else
                    node.Up = grid[row - 1].Count > column ? grid[row - 1][column] : grid[row - 1].Last();

                if (row == lastRow)
                {
                    if (column == 0 && _additionalNodeMap.ContainsKey(GridOuterPosition.BottomLeft))
                        node.Down = _additionalNodeMap[GridOuterPosition.BottomLeft];
                    else if (column == lastColumn && _additionalNodeMap.ContainsKey(GridOuterPosition.BottomRight))
                        node.Down = _additionalNodeMap[GridOuterPosition.BottomRight];
                    else 
                        node.Down = GetFirst(GridOuterPosition.BottomCenter, GridOuterPosition.BottomLeft, GridOuterPosition.BottomRight);
                }
                else
                    node.Down = grid[row + 1].Count > column ? grid[row + 1][column] : grid[row + 1].Last();

                if (column == 0)
                {
                    if (row == 0 && _additionalNodeMap.ContainsKey(GridOuterPosition.LeftTop))
                        node.Left = _additionalNodeMap[GridOuterPosition.LeftTop];
                    else if (row == lastRow && _additionalNodeMap.ContainsKey(GridOuterPosition.LeftBottom))
                        node.Left = _additionalNodeMap[GridOuterPosition.LeftBottom];
                    else
                        node.Left = GetFirst(GridOuterPosition.LeftCenter, GridOuterPosition.LeftTop, GridOuterPosition.LeftBottom);
                }
                else
                    node.Left = grid[row][column - 1];

                if (column == lastColumn)
                {
                    if (row == 0 && _additionalNodeMap.ContainsKey(GridOuterPosition.RightTop))
                        node.Right = _additionalNodeMap[GridOuterPosition.RightTop];
                    else if (row == lastRow && _additionalNodeMap.ContainsKey(GridOuterPosition.RightBottom))
                        node.Right = _additionalNodeMap[GridOuterPosition.RightBottom];
                    else
                        node.Right = GetFirst(GridOuterPosition.RightCenter, GridOuterPosition.RightTop, GridOuterPosition.RightBottom);
                }
                else
                    node.Right = grid[row][column + 1];
                    
                nodes.Add(node);
            }
        }

        foreach (var keyValue in _additionalNodeMap)
        {
            if (keyValue.Key == GridOuterPosition.TopLeft)
            {
                var node = new DirectionalInputNode { Selectable = keyValue.Value };
                node.Right = GetFirst(GridOuterPosition.TopCenter, GridOuterPosition.TopRight, GridOuterPosition.RightTop, GridOuterPosition.RightCenter, GridOuterPosition.RightBottom);
                node.Left = GetFirst(GridOuterPosition.LeftTop, GridOuterPosition.LeftCenter, GridOuterPosition.LeftBottom);
                node.Down = grid[0][0];
                nodes.Add(node);
            }
            else if (keyValue.Key == GridOuterPosition.TopCenter)
            {
                var node = new DirectionalInputNode { Selectable = keyValue.Value };
                node.Right = GetFirst(GridOuterPosition.TopRight, GridOuterPosition.RightTop, GridOuterPosition.RightCenter, GridOuterPosition.RightBottom);
                node.Left = GetFirst(GridOuterPosition.TopLeft, GridOuterPosition.LeftTop, GridOuterPosition.LeftCenter, GridOuterPosition.LeftBottom);
                node.Down = grid[0][(int)Math.Floor(grid[0].Count / 2m)];
                nodes.Add(node);
            }
            else if (keyValue.Key == GridOuterPosition.TopRight)
            {
                var node = new DirectionalInputNode { Selectable = keyValue.Value };
                node.Right = GetFirst(GridOuterPosition.RightTop, GridOuterPosition.RightCenter, GridOuterPosition.RightBottom);
                node.Left = GetFirst(GridOuterPosition.TopCenter, GridOuterPosition.TopLeft, GridOuterPosition.LeftTop, GridOuterPosition.LeftCenter, GridOuterPosition.LeftBottom);
                node.Down = grid[0].Last();
                nodes.Add(node);
            }
            else if (keyValue.Key == GridOuterPosition.BottomLeft)
            {
                var node = new DirectionalInputNode { Selectable = keyValue.Value };
                node.Right = GetFirst(GridOuterPosition.BottomCenter, GridOuterPosition.BottomRight, GridOuterPosition.RightBottom, GridOuterPosition.RightCenter, GridOuterPosition.RightTop);
                node.Left = GetFirst(GridOuterPosition.LeftBottom, GridOuterPosition.LeftCenter, GridOuterPosition.LeftTop);
                node.Up = grid.Last()[0];
                nodes.Add(node);
            }
            else if (keyValue.Key == GridOuterPosition.BottomCenter)
            {
                var node = new DirectionalInputNode { Selectable = keyValue.Value };
                node.Right = GetFirst(GridOuterPosition.BottomRight, GridOuterPosition.RightBottom, GridOuterPosition.RightCenter, GridOuterPosition.RightTop);
                node.Left = GetFirst(GridOuterPosition.BottomLeft, GridOuterPosition.LeftBottom, GridOuterPosition.LeftCenter, GridOuterPosition.LeftTop);
                node.Up = grid.Last()[(int)Math.Floor(grid.Last().Count / 2m)];
                nodes.Add(node);
            }
            else if (keyValue.Key == GridOuterPosition.BottomRight)
            {
                var node = new DirectionalInputNode { Selectable = keyValue.Value };
                node.Right = GetFirst(GridOuterPosition.RightBottom, GridOuterPosition.RightCenter, GridOuterPosition.RightTop);
                node.Left = GetFirst(GridOuterPosition.BottomCenter, GridOuterPosition.BottomLeft, GridOuterPosition.LeftBottom, GridOuterPosition.LeftCenter, GridOuterPosition.LeftTop);
                node.Up = grid.Last().Last();
                nodes.Add(node);
            }
            else if (keyValue.Key == GridOuterPosition.LeftTop)
            {
                var node = new DirectionalInputNode { Selectable = keyValue.Value };
                node.Up = GetFirst(GridOuterPosition.TopLeft, GridOuterPosition.TopCenter, GridOuterPosition.TopRight);
                node.Down = GetFirst(GridOuterPosition.LeftCenter, GridOuterPosition.LeftBottom, GridOuterPosition.BottomLeft, GridOuterPosition.BottomCenter, GridOuterPosition.BottomRight);
                node.Right = grid[0][0];
                nodes.Add(node);
            }
            else if (keyValue.Key == GridOuterPosition.LeftCenter)
            {
                var node = new DirectionalInputNode { Selectable = keyValue.Value };
                node.Up = GetFirst(GridOuterPosition.LeftTop, GridOuterPosition.TopLeft, GridOuterPosition.TopCenter, GridOuterPosition.TopRight);
                node.Down = GetFirst(GridOuterPosition.LeftBottom, GridOuterPosition.BottomLeft, GridOuterPosition.BottomCenter, GridOuterPosition.BottomRight);
                node.Right = grid[(int)Math.Floor(grid.Count / 2m)][0];
                nodes.Add(node);
            }
            else if (keyValue.Key == GridOuterPosition.LeftBottom)
            {
                var node = new DirectionalInputNode { Selectable = keyValue.Value };
                node.Up = GetFirst(GridOuterPosition.LeftCenter, GridOuterPosition.LeftTop, GridOuterPosition.TopLeft, GridOuterPosition.TopCenter, GridOuterPosition.TopRight);
                node.Down = GetFirst(GridOuterPosition.BottomLeft, GridOuterPosition.BottomCenter, GridOuterPosition.BottomRight);
                node.Right = grid.Last()[0];
                nodes.Add(node);
            }
            else if (keyValue.Key == GridOuterPosition.RightTop)
            {
                var node = new DirectionalInputNode { Selectable = keyValue.Value };
                node.Up = GetFirst(GridOuterPosition.TopRight, GridOuterPosition.TopCenter, GridOuterPosition.TopLeft);
                node.Down = GetFirst(GridOuterPosition.RightCenter, GridOuterPosition.RightBottom, GridOuterPosition.BottomRight, GridOuterPosition.BottomCenter, GridOuterPosition.BottomLeft);
                node.Left = grid[0].Last();
                nodes.Add(node);
            }
            else if (keyValue.Key == GridOuterPosition.RightCenter)
            {
                var node = new DirectionalInputNode { Selectable = keyValue.Value };
                node.Up = GetFirst(GridOuterPosition.RightTop, GridOuterPosition.TopRight, GridOuterPosition.TopCenter, GridOuterPosition.TopLeft);
                node.Down = GetFirst(GridOuterPosition.RightBottom, GridOuterPosition.BottomRight, GridOuterPosition.BottomCenter, GridOuterPosition.BottomLeft);
                node.Left = grid[(int)Math.Floor(grid.Count / 2m)].Last();
                nodes.Add(node);
            }
            else if (keyValue.Key == GridOuterPosition.RightBottom)
            {
                var node = new DirectionalInputNode { Selectable = keyValue.Value };
                node.Up = GetFirst(GridOuterPosition.RightCenter, GridOuterPosition.RightTop, GridOuterPosition.TopRight, GridOuterPosition.TopCenter, GridOuterPosition.TopLeft);
                node.Down = GetFirst(GridOuterPosition.BottomRight, GridOuterPosition.BottomCenter, GridOuterPosition.BottomLeft);
                node.Left = grid.Last().Last();
                nodes.Add(node);
            }
        }

        _nodeMap = new DirectionalInputNodeMap
        {
            Z = z,
            DefaultSelected = new[] { nodes[0].Selectable },
            BackObject = back,
            Nodes = nodes.ToArray()
        };
        
        Message.Publish(new DirectionalInputNodeMapEnabled(_nodeMap));
    }

    private GameObject GetFirst(params GridOuterPosition[] preferred)
    {
        foreach (var position in preferred)
            if (_additionalNodeMap.ContainsKey(position))
                return _additionalNodeMap[position];
        return null;
    }

    private void OnDisable()
        => Message.Publish(new DirectionalInputNodeMapDisabled(_nodeMap));
}