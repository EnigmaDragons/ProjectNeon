using UnityEngine;

namespace Features.GameProgression
{
    [CreateAssetMenu(menuName = "OnlyOnce/CurrentAdventureProgress")]
    public class CurrentAdventureProgress : ScriptableObject
    {
        public AdventureProgressBase AdventureProgress;
    }
}