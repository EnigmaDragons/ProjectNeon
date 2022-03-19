using System;
using System.Collections.Generic;
using System.Linq;

public class ColumnConnectionGenerator
{
    public IEnumerable<IEnumerable<Connection>> GeneratePossibilities(
        IEnumerable<MapNodeInColumn> fromNodes,
        IEnumerable<MapNodeInColumn> toNodes)
    {
        fromNodes = fromNodes.ToArray();
        toNodes = toNodes.ToArray();
        if (!fromNodes.Any() || !toNodes.Any())
            return new List<Connection[]>();
        var connectionMap = new Dictionary<MapNodeInColumn, Dictionary<MapNodeInColumn, bool>>();
        foreach (var fromNode in fromNodes)
        {
            var possibleChildren = new List<MapNodeInColumn>();
            foreach (var toNode in toNodes.OrderBy(x => x.Y))
            {
                if (fromNode.Y < toNode.Y)
                {
                    possibleChildren.Add(toNode);
                    break;
                }
                else if (fromNode.Y == toNode.Y)
                    possibleChildren.Add(toNode);
                else
                    possibleChildren = new List<MapNodeInColumn> { toNode };
            }
            connectionMap[fromNode] = new Dictionary<MapNodeInColumn, bool>();
            foreach (var child in possibleChildren)
                connectionMap[fromNode][child] = possibleChildren.Count == 1 || child.Y == fromNode.Y;
        }
        foreach (var toNode in toNodes)
        {
            var possibleParents = new List<MapNodeInColumn>();
            foreach (var fromNode in fromNodes.OrderBy(x => x.Y))
            {
                if (toNode.Y < fromNode.Y)
                {
                    possibleParents.Add(fromNode);
                    break;
                }
                else if (toNode.Y == fromNode.Y)
                    possibleParents.Add(fromNode);
                else
                    possibleParents = new List<MapNodeInColumn> { fromNode };
            }
            foreach (var parent in possibleParents)
                connectionMap[parent][toNode] = (connectionMap[parent].ContainsKey(toNode) && connectionMap[parent][toNode]) || possibleParents.Count == 1;
        }
        var connections = new List<Connection[]>();
        RecursivelyReduceToFindAllPossibilities(fromNodes, toNodes, connections, connectionMap);
        return connections;
    }

    private void RecursivelyReduceToFindAllPossibilities(
        IEnumerable<MapNodeInColumn> fromNodes,
        IEnumerable<MapNodeInColumn> toNodes,
        List<Connection[]> possibilities, 
        Dictionary<MapNodeInColumn, Dictionary<MapNodeInColumn, bool>> currentConnections)
    {
        if (IsUnconnectedNodePresent(fromNodes, toNodes, currentConnections))
            return;
        var possibility = currentConnections.SelectMany(from => from.Value.Select(to => new Connection { From = from.Key, To = to.Key })).ToArray();
        if (HasNoCrossingLine(possibility))
        {
            if (possibilities.Any(x => AreEqual(x, possibility)))
                return;
            possibilities.Add(possibility);
        }
        currentConnections.ForEach(from => from.Value.Where(x => !x.Value).ForEach(to => RecursivelyReduceToFindAllPossibilities(fromNodes, toNodes, possibilities, currentConnections.With(from.Key, from.Value.Without(to.Key)))));
    }

    private bool IsUnconnectedNodePresent(
        IEnumerable<MapNodeInColumn> fromNodes,
        IEnumerable<MapNodeInColumn> toNodes,
        Dictionary<MapNodeInColumn, Dictionary<MapNodeInColumn, bool>> currentConnections)
    {
        var presentToNodes = new HashSet<MapNodeInColumn>();
        foreach (var fromNode in fromNodes)
        {
            if (currentConnections[fromNode].Count == 0)
                return true;
            currentConnections[fromNode].ForEach(x => presentToNodes.Add(x.Key));
        }
        return toNodes.Any(x => !presentToNodes.Contains(x));
    }

    private bool HasNoCrossingLine(Connection[] connections)
    {
        for (var i = 0; i < connections.Count(); i++)
        {
            for (var ii = i + 1; ii < connections.Count(); ii++)
            {
                if ((connections[i].From.Y > connections[ii].From.Y && connections[i].To.Y < connections[ii].To.Y)
                    || (connections[i].From.Y < connections[ii].From.Y && connections[i].To.Y > connections[ii].To.Y))
                    return false;
            }
        }
        return true;
    }

    private bool AreEqual(Connection[] possibility1, Connection[] possibility2)
    {
        if (possibility1.Length != possibility2.Length)
            return false;
        foreach (var connection in possibility1)
            if (!possibility2.Any(x => x.From.NodeId == connection.From.NodeId && x.To.NodeId == connection.To.NodeId))
                return false;
        return true;
    }
}