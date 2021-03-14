using UnityEngine;

class InitializeAdventureOnEnable : MonoBehaviour
{
    [SerializeField] private AdventureProgress2 adventure2;
    
    private void OnEnable()
    {
        adventure2.Reset();
        adventure2.InitIfNeeded();
    }
}
