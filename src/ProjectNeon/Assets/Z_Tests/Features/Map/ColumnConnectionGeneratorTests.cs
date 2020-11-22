using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

public class ColumnConnectionGeneratorTests
{
    private readonly ColumnConnectionGenerator _generator = new ColumnConnectionGenerator();

    [Test]
    public void GeneratePossibilities_NoNodes_NoPossibilities()
        => TestConnectionGenerator(new MapNodeInColumn[0], new MapNodeInColumn[0], new List<Tuple<int, int>[]>());

    [Test]
    public void GeneratePossibilities_NoFromNodes_NoPossibilities()
        => TestConnectionGenerator(new MapNodeInColumn[0], new [] { new MapNodeInColumn { NodeId = "1", Y = 1 } }, new List<Tuple<int, int>[]>());

    [Test]
    public void GeneratePossibilities_NoToNodes_NoPossibilities()
        => TestConnectionGenerator(new [] { new MapNodeInColumn { NodeId = "1", Y = 1 } }, new MapNodeInColumn[0], new List<Tuple<int, int>[]>());

    [Test]
    public void GeneratePossibilities_1FromAnd1To_OnePossibility()
        => TestConnectionGenerator(
            new [] { new MapNodeInColumn { NodeId = "1", Y = 1 } },
            new [] { new MapNodeInColumn { NodeId = "2", Y = 1 } },
            new List<Tuple<int, int>[]> { new Tuple<int, int>[] { new Tuple<int, int>(1, 2) }});
    
    [Test]
    public void GeneratePossibilities_1FromAnd2To_OnePossibility()
        => TestConnectionGenerator(
            new [] { new MapNodeInColumn { NodeId = "1", Y = 1 } },
            new [] { new MapNodeInColumn { NodeId = "2", Y = 1 }, new MapNodeInColumn { NodeId = "3", Y = 2 } },
            new List<Tuple<int, int>[]> { new Tuple<int, int>[] { new Tuple<int, int>(1, 2), new Tuple<int, int>(1, 3) }});
    
    [Test]
    public void GeneratePossibilities_2FromAnd1To_OnePossibility()
        => TestConnectionGenerator(
            new [] { new MapNodeInColumn { NodeId = "1", Y = 1 }, new MapNodeInColumn { NodeId = "2", Y = 2 } },
            new [] { new MapNodeInColumn { NodeId = "3", Y = 1 }},
            new List<Tuple<int, int>[]> { new Tuple<int, int>[] { new Tuple<int, int>(1, 3), new Tuple<int, int>(2, 3) }});
    
    [Test]
    public void GeneratePossibilities_2FromAnd2ToPerfectlyLinedUp_ThreePossibilities()
        => TestConnectionGenerator(
            new [] { new MapNodeInColumn { NodeId = "1", Y = 1 }, new MapNodeInColumn { NodeId = "2", Y = 2 } },
            new [] { new MapNodeInColumn { NodeId = "3", Y = 1 }, new MapNodeInColumn { NodeId = "4", Y = 2 } },
            new List<Tuple<int, int>[]>
            {
                new Tuple<int, int>[] { new Tuple<int, int>(1, 3), new Tuple<int, int>(2, 4) },
                new Tuple<int, int>[] { new Tuple<int, int>(1, 3), new Tuple<int, int>(1, 4),  new Tuple<int, int>(2, 4) },
                new Tuple<int, int>[] { new Tuple<int, int>(1, 3), new Tuple<int, int>(2, 3), new Tuple<int, int>(2, 4) },
            });

    [Test]
    public void GeneratePossibilities_2FromAnd2ToDiamondShape_TwoPossibilities()
        => TestConnectionGenerator(
            new [] { new MapNodeInColumn { NodeId = "1", Y = 1 }, new MapNodeInColumn { NodeId = "2", Y = 3 } },
            new [] { new MapNodeInColumn { NodeId = "3", Y = 2 }, new MapNodeInColumn { NodeId = "4", Y = 4 } },
            new List<Tuple<int, int>[]>
            {
                new Tuple<int, int>[] { new Tuple<int, int>(1, 3), new Tuple<int, int>(2, 4) },
                new Tuple<int, int>[] { new Tuple<int, int>(1, 3), new Tuple<int, int>(2, 4), new Tuple<int, int>(2, 3),  },
            });
    
    [Test]
    public void GeneratePossibilities_2FromOuterAnd2ToInner_ThreePossibilities()
        => TestConnectionGenerator(
            new [] { new MapNodeInColumn { NodeId = "1", Y = 1 }, new MapNodeInColumn { NodeId = "2", Y = 4 } },
            new [] { new MapNodeInColumn { NodeId = "3", Y = 2 }, new MapNodeInColumn { NodeId = "4", Y = 3 } },
            new List<Tuple<int, int>[]>
            {
                new Tuple<int, int>[] { new Tuple<int, int>(1, 3), new Tuple<int, int>(2, 4) },
                new Tuple<int, int>[] { new Tuple<int, int>(1, 3), new Tuple<int, int>(1, 4),  new Tuple<int, int>(2, 4) },
                new Tuple<int, int>[] { new Tuple<int, int>(1, 3), new Tuple<int, int>(2, 3), new Tuple<int, int>(2, 4) },
            });
    
    [Test]
    public void GeneratePossibilities_2FromInnerAnd2ToOuter_ThreePossibilities()
        => TestConnectionGenerator(
            new [] { new MapNodeInColumn { NodeId = "1", Y = 2 }, new MapNodeInColumn { NodeId = "2", Y = 3 } },
            new [] { new MapNodeInColumn { NodeId = "3", Y = 1 }, new MapNodeInColumn { NodeId = "4", Y = 4 } },
            new List<Tuple<int, int>[]>
            {
                new Tuple<int, int>[] { new Tuple<int, int>(1, 3), new Tuple<int, int>(2, 4) },
                new Tuple<int, int>[] { new Tuple<int, int>(1, 3), new Tuple<int, int>(1, 4),  new Tuple<int, int>(2, 4) },
                new Tuple<int, int>[] { new Tuple<int, int>(1, 3), new Tuple<int, int>(2, 3), new Tuple<int, int>(2, 4) },
            });
    
    [Test]
    public void GeneratePossibilities_2FromOuterAnd3ToInner_FivePossibilities()
        => TestConnectionGenerator(
            new [] { new MapNodeInColumn { NodeId = "1", Y = 1 }, new MapNodeInColumn { NodeId = "2", Y = 5 } },
            new [] { new MapNodeInColumn { NodeId = "3", Y = 2 }, new MapNodeInColumn { NodeId = "4", Y = 3 }, new MapNodeInColumn { NodeId = "5", Y = 4 } },
            new List<Tuple<int, int>[]>
            {
                new Tuple<int, int>[] { new Tuple<int, int>(1, 3), new Tuple<int, int>(2, 5), new Tuple<int, int>(1, 4) },
                new Tuple<int, int>[] { new Tuple<int, int>(1, 3), new Tuple<int, int>(2, 5), new Tuple<int, int>(2, 4) },
                new Tuple<int, int>[] { new Tuple<int, int>(1, 3), new Tuple<int, int>(2, 5), new Tuple<int, int>(1, 4), new Tuple<int, int>(1, 5) },
                new Tuple<int, int>[] { new Tuple<int, int>(1, 3), new Tuple<int, int>(2, 5), new Tuple<int, int>(1, 4), new Tuple<int, int>(2, 4) },
                new Tuple<int, int>[] { new Tuple<int, int>(1, 3), new Tuple<int, int>(2, 5), new Tuple<int, int>(2, 3), new Tuple<int, int>(2, 4) },
            });
    
    [Test]
    public void GeneratePossibilities_2FromInnerAnd4ToOuter_ThreePossibilities()
        => TestConnectionGenerator(
            new [] { new MapNodeInColumn { NodeId = "1", Y = 3 }, new MapNodeInColumn { NodeId = "2", Y = 4 } },
            new [] { new MapNodeInColumn { NodeId = "3", Y = 1 }, new MapNodeInColumn { NodeId = "4", Y = 2 }, new MapNodeInColumn { NodeId = "5", Y = 5 }, new MapNodeInColumn { NodeId = "6", Y = 6 }  },
            new List<Tuple<int, int>[]>
            {
                new Tuple<int, int>[] { new Tuple<int, int>(1, 3), new Tuple<int, int>(1, 4), new Tuple<int, int>(2, 5), new Tuple<int, int>(2, 6) },
                new Tuple<int, int>[] { new Tuple<int, int>(1, 3), new Tuple<int, int>(1, 4), new Tuple<int, int>(2, 5), new Tuple<int, int>(2, 6), new Tuple<int, int>(1, 5) },
                new Tuple<int, int>[] { new Tuple<int, int>(1, 3), new Tuple<int, int>(1, 4), new Tuple<int, int>(2, 5), new Tuple<int, int>(2, 6), new Tuple<int, int>(2, 4) },
            });

    private void TestConnectionGenerator(MapNodeInColumn[] fromNodes, MapNodeInColumn[] toNodes, List<Tuple<int, int>[]> expectedPossibilities)
    {
        var actualPossibilities = _generator.GeneratePossibilities(fromNodes, toNodes);
        
        Assert.AreEqual(expectedPossibilities.Count, actualPossibilities.Count());
        foreach (var expectedPossibility in expectedPossibilities)
            Assert.IsTrue(actualPossibilities.Any(actualPossibility =>
            {
                if (actualPossibility.Count() != expectedPossibility.Length)
                    return false;
                foreach (var expectedConnection in expectedPossibility)
                    if (!actualPossibility.Any(actualConnection => actualConnection.From.NodeId == expectedConnection.Item1.ToString() && actualConnection.To.NodeId == expectedConnection.Item2.ToString()))
                        return false;
                return true;
            }));
    }
}