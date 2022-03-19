using System.Collections.Generic;
using System.Linq;

public class NoNonCombatDuplicates : MapGenerationRule
{
    public override List<MapNodeType> FilterNodeTypes(List<MapNodeType> remainingTypes, List<List<MapNode>> map, MapNode node, int column, List<MapNode> toNodes)
        => remainingTypes.Where(type => type == MapNodeType.Combat || type == MapNodeType.Elite || toNodes.All(to => to.Type != type)).ToList();
}