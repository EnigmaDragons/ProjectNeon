using System;
using UnityEngine;

[Obsolete("BattleV1")]
public class SelectionCursorVisualizer : MonoBehaviour
{
    [SerializeField] private GameEvent activateOn;
    [SerializeField] private GameEvent deactivateOn;
    [SerializeField] private GameObject cursor;
    [SerializeField] private BattlePlayerTargetingState targeting;
    [SerializeField] private BattleState battleState;
    [SerializeField] private Vector3 enemyOffset;
    [SerializeField] private Vector3 partyOffset;

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
        if (firstTarget.Members.Length == 0) return;
        
        var firstMember = firstTarget.Members[0];
        cursor.transform.position = battleState.GetTransform(firstMember.Id).position + Offset(firstMember);
        if (firstTarget.Members.Length <= 1) return;
        extraCursors = new GameObject[firstTarget.Members.Length - 1];
        for (var i = 1; i < firstTarget.Members.Length; i++)
        {
            extraCursors[i - 1] = Instantiate(cursor, 
                battleState.GetTransform(firstTarget.Members[i].Id).position + Offset(firstTarget.Members[i]), 
                cursor.transform.rotation, 
                transform);
            extraCursors[i - 1].SetActive(false);
        }
    }

    private Vector3 Offset(Member m) => m.TeamType == TeamType.Enemies ? enemyOffset : partyOffset;

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
