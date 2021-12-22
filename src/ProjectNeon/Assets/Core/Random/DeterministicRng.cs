using System;
using System.Collections.Generic;
using System.Linq;

public class DeterministicRng
{
    private readonly Random _instance;
    
    public int Seed { get; }
    
    public DeterministicRng(int seed)
    {
        Seed = seed;
        _instance = new Random(seed);
    }

    public bool Bool() => Int(2) == 1;
    public bool Chance(double percent) => _instance.NextDouble() < percent;
    public int Int() => Int(int.MaxValue);
    public int Int(int max) => _instance.Next(max);
    public int Int(int min, int max) => _instance.Next(min, max);
    public double Dbl() => _instance.NextDouble();
    public KeyValuePair<T, T2> Random<T, T2>(Dictionary<T, T2> dictionary) => dictionary.ElementAt(Int(dictionary.Count));
    public T Random<T>(T[] array) => array[Int(array.Length)];
    public T Random<T>(List<T> list) => list[Int(list.Count)];
    public T Random<T>(Array array) => (T) array.GetValue(Int(array.Length));
    public T Random<T>(IEnumerable<T> items) => items.ToArray().Random();

    public T DrawRandom<T>(List<T> list)
    {
        var roll = Int(list.Count);
        var item = list[roll];
        list.RemoveAt(roll);
        return item;
    }

    public T[] Shuffled<T>(T[] arr)
    {
        var shuffled = arr.ToArray();
        var i = shuffled.Length;
        while (i > 1) 
        {
            var k = _instance.Next(i--);
            var temp = shuffled[i];
            shuffled[i] = shuffled[k];
            shuffled[k] = temp;
        }
        return shuffled;
    }
    
    public List<T> Shuffled<T>(List<T> list)
    {
        var shuffled = list.ToList();
        var i = shuffled.Count;
        while (i > 1) 
        {
            var k = _instance.Next(i--);
            var temp = shuffled[i];
            shuffled[i] = shuffled[k];
            shuffled[k] = temp;
        }
        return shuffled;
    }

    public T[] TakeRandom<T>(IEnumerable<T> items, int number)
        => items
            .ToArray()
            .Shuffled()
            .Take(number)
            .ToArray();
}