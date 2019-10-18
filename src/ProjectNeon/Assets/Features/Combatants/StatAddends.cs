using System;

namespace Features.Combatants
{
    public sealed class StatAddends : IStats
    {
        public float MaxHP { get; set; }
        public float MaxShield { get; set; }
        public float Attack { get; set; }
        public float Magic { get; set;  }
        public float Armor { get; set; }
        public float Resistance { get; set; }
        public IResourceType[] ResourceTypes { get; set; } = new IResourceType[0];
        public Func<int, bool> ActiveFunction = (currentTurn) => true;
        public bool Active(int currentTurn) { return ActiveFunction.Invoke(currentTurn); }
    }
}
