using System.Collections.Generic;
using System.Linq;

public class ConnectionGenerator
{
    private readonly ColumnConnectionGenerator _columnConnectionGenerator = new ColumnConnectionGenerator();
    
    public void AddConnections(List<List<MapNode>> columns)
    {
        for (var i = 0; i < columns.Count - 1; i++)
        {
            var fromNodes = columns[i];
            var toNodes = columns[i + 1];
            var possibilities = _columnConnectionGenerator.GeneratePossibilities(
                columns[i].Select(x => new MapNodeInColumn { NodeId = x.NodeId, Y = x.Y }), 
                columns[i + 1].Select(x => new MapNodeInColumn { NodeId = x.NodeId, Y = x.Y }));
            //Add additional possibility selection here
            var possibility = possibilities.Random();
            foreach (var connection in possibility)
                fromNodes.First(x => x.NodeId == connection.From.NodeId).ChildrenIds.Add(toNodes.First(x => x.NodeId == connection.To.NodeId).NodeId);
        }
    }
}