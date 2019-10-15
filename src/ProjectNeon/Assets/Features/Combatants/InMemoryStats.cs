using System;

namespace Features.Combatants
{
    public sealed class InMemoryStats : IStats
    {
        public int MaxHP { get; set; }
        public int MaxShield { get; set; }
        public int Attack { get; set; }
        public int Magic { get; set;  }
        public float Armor { get; set; }
        public float Resistance { get; set; }
        public IResourceType[] ResourceTypes { get; set; } = new IResourceType[0];
        public Func<int, bool> active;
        public bool Active(int currentTurn) { return active.Invoke(currentTurn); }
    }
}
