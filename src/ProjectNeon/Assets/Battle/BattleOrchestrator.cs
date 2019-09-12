using UnityEngine;

public sealed class BattleOrchestrator : MonoBehaviour
{
    [SerializeField] private GameEvent OnStart;
    [SerializeField] private GameEventListener OnSetupBegun;
    [SerializeField] private string CurrentPhase = "Not Initialized";
    
    void Start()
    {
        OnStart.Publish();
    }

    public void SetCurrentPhaseDescription(string description) => CurrentPhase = description;
}
