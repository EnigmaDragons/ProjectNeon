using System;
using System.Linq;

namespace Features.GameProgression
{
    [Serializable]
    public sealed class SaveState
    {
        public bool IsValid => Party != null;
        public string VersionNumber;
        public PartySaveState Party;
    }

    [Serializable]
    public sealed class PartySaveState
    {
        public HeroSaveState[] Heroes = new HeroSaveState[0];
        public int NumCredits;
        public int NumShopRestocks;
    }

    [Serializable]
    public sealed class HeroSaveState
    {
        public string Name;
    }

    public static class SaveStateConverters
    {
        public static SaveState Create(string versionNumber, PartyAdventureState s)
            => new SaveState
            {
                VersionNumber = versionNumber,
                Party = new PartySaveState
                {
                    NumCredits = s.Credits,
                    NumShopRestocks = s.NumShopRestocks,
                    Heroes = s.Heroes.Select(h => new HeroSaveState
                    {
                        Name = h.Name
                    }).ToArray()
                }
            };
    }
}
