
using Features.GameProgression;
using UnityEngine;

class InitializeAdventureOnEnable : MonoBehaviour
{
    [SerializeField] private CurrentAdventure currentAdventure;
    [SerializeField] private AdventureProgress adventure;
    [SerializeField] private AdventureProgress2 adventure2;
    
    private void OnEnable()
    {
        if (currentAdventure.Adventure.IsV2)
        {
            adventure2.Reset();
            adventure2.InitIfNeeded();
        }
        else
        {
            adventure.Reset();
            adventure.InitIfNeeded();    
        }
    }
}
