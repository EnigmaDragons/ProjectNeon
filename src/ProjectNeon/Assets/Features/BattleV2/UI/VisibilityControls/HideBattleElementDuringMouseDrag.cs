using UnityEngine;

public class HideBattleElementDuringMouseDrag : OnMessage<MouseDragStateChanged>
{
    private const string _callerId = "HideBattleElementDuringMouseDrag";

    [SerializeField] private string[] targets;

    protected override void Execute(MouseDragStateChanged msg)
        => targets.ForEach(t => Message.Publish(new SetBattleUiElementVisibility(t, !msg.IsDragging, _callerId)));
}