using UnityEngine;

namespace Features.GameProgression
{
    [CreateAssetMenu(menuName = "OnlyOnce/CurrentAdventureProgress")]
    public class CurrentAdventureProgress : ScriptableObject
    {
        public bool HasActiveAdventure => AdventureProgress != null;
        public AdventureProgressBase AdventureProgress;
    }
}