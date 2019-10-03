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
    private GameObject[] extraCursors = new GameObject[0];
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
        cursor.transform.position = battleState.GetPosition(firstMember.Id) + offset;
        if (firstTarget.Members.Length <= 1) return;
        extraCursors = new GameObject[firstTarget.Members.Length - 1];
        for (int i = 1; i < firstTarget.Members.Length; i++)
        {
            extraCursors[i - 1] = Instantiate(cursor, battleState.GetPosition(firstTarget.Members[i].Id) + offset, cursor.transform.rotation);
        }
    }

    void Activate()
    {
        isActive = true;
        cursor.SetActive(isActive);
        ExtraCursorsAction(isActive);
    }

    void Deactivate()
    {
        isActive = false;
        cursor.SetActive(isActive);
        ExtraCursorsAction(isActive);
    }
    void ExtraCursorsAction(bool needCursors)
    {
        if (extraCursors.Length == 0) return;
        if (needCursors) extraCursors.ForEach(x => x.SetActive(true));
        else
        {
            extraCursors.ForEach(x => Destroy(x));
            extraCursors = new GameObject[0];
        }

    }
}
