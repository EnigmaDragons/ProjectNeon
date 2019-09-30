using UnityEngine;

public class SelectionCursorVisualizer : MonoBehaviour
{
    [SerializeField] private GameEvent activateOn;
    [SerializeField] private GameEvent deactivateOn;
    [SerializeField] private GameObject cursor;
    [SerializeField] private BattlePlayerTargetingState targeting;
    [SerializeField] private BattleState battleState;
    [SerializeField] private Vector3 offset;

    private bool isActive;

    private void OnEnable()
    {
        activateOn.Subscribe(Activate, this);
        deactivateOn.Subscribe(Deactivate, this);
        targeting.OnTargetChanged.Subscribe(UpdateTarget, this);
    }
    
    private void OnDisable()
    {
        activateOn.Unsubscribe(this);
        deactivateOn.Unsubscribe(this);
        targeting.OnTargetChanged.Unsubscribe(this);
    }

    void UpdateTarget()
    {
        var firstTarget = targeting.Current;
        var firstMember = firstTarget.Members[0];
        // @todo #1:30min Visualize Multi-Targets
        cursor.transform.position = battleState.GetPosition(firstMember.Id) + offset;
    }

    void Activate()
    {
        isActive = true;
        cursor.SetActive(isActive);
    }

    void Deactivate()
    {
        isActive = false;
        cursor.SetActive(isActive);
    }
}
