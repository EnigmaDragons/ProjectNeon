using System;
using UnityEngine;

[Obsolete("Reference Only. New version may be used on Consoles.")]
public class SelectionCursorV2 : OnMessage<TargetSelectionBegun, TargetSelectionFinished, TargetChanged>
{
    [SerializeField] private GameObject cursor;
    [SerializeField] private BattleState battleState;
    [SerializeField] private Vector3 enemyOffset;
    [SerializeField] private Vector3 partyOffset;

    private bool _isActive;
    private GameObject[] _extraCursors = new GameObject[0];
    
    protected override void Execute(TargetSelectionBegun msg) => Activate();
    protected override void Execute(TargetSelectionFinished msg) => Deactivate();
    
    protected override void Execute(TargetChanged msg)
    {
        msg.Target.IfNull(Deactivate);
        msg.Target.IfPresent(UpdateTarget);
    }

    void UpdateTarget(Target firstTarget)
    {
        // if (firstTarget.Members.Length == 0) return;
        //
        // var firstMember = firstTarget.Members[0];
        // cursor.transform.position = battleState.GetTransform(firstMember.Id).position + Offset(firstMember);
        // if (firstTarget.Members.Length <= 1) return;
        // _extraCursors = new GameObject[firstTarget.Members.Length - 1];
        // for (var i = 1; i < firstTarget.Members.Length; i++)
        // {
        //     _extraCursors[i - 1] = Instantiate(cursor, 
        //         battleState.GetTransform(firstTarget.Members[i].Id).position + Offset(firstTarget.Members[i]), 
        //         cursor.transform.rotation, 
        //         transform);
        //     _extraCursors[i - 1].SetActive(false);
        // }
    }

    private Vector3 Offset(Member m) => m.TeamType == TeamType.Enemies ? enemyOffset : partyOffset;

    void Activate()
    {
        _isActive = true;
        cursor.SetActive(_isActive);
        ExtraCursorsAction(_isActive);
    }

    void Deactivate()
    {
        _isActive = false;
        cursor.SetActive(_isActive);
        ExtraCursorsAction(_isActive);
    }
    
    void ExtraCursorsAction(bool needCursors)
    {
        if (_extraCursors.Length == 0) return;
        if (needCursors) _extraCursors.ForEach(x => x.SetActive(true));
        else
        {
            _extraCursors.ForEach(Destroy);
            _extraCursors = new GameObject[0];
        }
    }
}
