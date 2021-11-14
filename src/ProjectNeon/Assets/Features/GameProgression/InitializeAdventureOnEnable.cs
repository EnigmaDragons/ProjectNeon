using Features.GameProgression;
using UnityEngine;

class InitializeAdventureOnEnable : MonoBehaviour
{
    [SerializeField] private CurrentAdventureProgress adventure;
    
    private void OnEnable()
    {
        adventure.AdventureProgress.Reset();
        adventure.AdventureProgress.InitIfNeeded();
    }
}
