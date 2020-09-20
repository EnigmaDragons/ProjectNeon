
using UnityEngine;

class InitializeAdventureOnEnable : MonoBehaviour
{
    [SerializeField] private AdventureProgress adventure;

    private void OnEnable()
    {
        adventure.Reset();
        adventure.InitIfNeeded();
    }
}
