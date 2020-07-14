using System;
using System.Collections;
using System.Collections.Generic;

public sealed class TwoDimensionalIterator : IEnumerable<Tuple<int, int>>
{
    private readonly int _xSize;
    private readonly int _ySize;

    public TwoDimensionalIterator(int xSize, int ySize)
    {
        _xSize = xSize;
        _ySize = ySize;
    }

    private IEnumerable<Tuple<int, int>> All()
    {
        for (var x = 0; x < _xSize; x++)
            for (var y = 0; y < _ySize; y++)
                yield return new Tuple<int, int>(x, y);
    }

    public IEnumerator<Tuple<int, int>> GetEnumerator() => All().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
