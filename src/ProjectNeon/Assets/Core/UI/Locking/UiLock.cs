using System.Collections.Generic;
using System.Linq;

public static class UiLock
{
    private static readonly HashSet<int> _locks = new HashSet<int>();

    public static void Init() => _locks.Clear();
    public static bool IsLocked => _locks.Any();
    public static void Add(int uniqueLockId) => _locks.Add(uniqueLockId);
    public static void Remove(int uniqueLockId) => _locks.Remove(uniqueLockId);
}
