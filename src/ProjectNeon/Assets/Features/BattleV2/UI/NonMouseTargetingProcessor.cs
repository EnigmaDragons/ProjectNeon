using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NonMouseTargetingProcessor : OnMessage<BeginTargetSelectionRequested, TargetingStateUpdated, EndTargetSelectionRequested, CancelTargetSelectionRequested>
{
    [SerializeField] private int z;
    [SerializeField] private BattlePlayerTargetingStateV2 targetingState;
    [SerializeField] private GameObject parent;
    [SerializeField] private Button backButton;
    [SerializeField] private NonMouseTargetable targetable;

    private bool _targeting;
    private DirectionalInputNodeMap _nodeMap;
    private bool _requiresTargeting;
    private CardPresenter _presenter;

    private void Awake() => backButton.onClick.AddListener(() => Message.Publish(new CancelTargetSelectionRequested()));

    protected override void Execute(BeginTargetSelectionRequested msg)
    {
        _requiresTargeting = msg.Card.RequiresPlayerTargeting();
        _presenter = msg.CardPresenter;
    }

    protected override void Execute(TargetingStateUpdated msg)
    {
        if (_requiresTargeting)
        {
            _targeting = true;
            parent.DestroyAllChildren();
            var nodes = new List<DirectionalInputNode>();
            foreach (var member in targetingState.MembersToIndicate)
            {
                var obj = Instantiate(targetable, parent.transform);
                obj.Init(_presenter, member);
                nodes.Add(new DirectionalInputNode { Selectable = obj.gameObject });
            }
            for (var i = 0; i < nodes.Count; i++)
            {
                nodes[i].Up = nodes[i == 0 ? nodes.Count - 1 : i - 1].Selectable;
                nodes[i].Down = nodes[i == nodes.Count - 1 ? 0 : i + 1].Selectable;
                nodes[i].Left = nodes[i == 0 ? nodes.Count - 1 : i - 1].Selectable;
                nodes[i].Right = nodes[i == nodes.Count - 1 ? 0 : i + 1].Selectable;
            }
            _nodeMap = new DirectionalInputNodeMap
            {
                Z = z,
                BackObject = backButton.gameObject,
                DefaultSelected = nodes.Select(x => x.Selectable).ToArray(),
                Nodes = nodes.ToArray()
            };
            Message.Publish(new DirectionalInputNodeMapEnabled(_nodeMap));
        }
        else if (InputControl.Type != ControlType.Mouse)
            Message.Publish(new EndTargetSelectionRequested(false));
    }

    protected override void Execute(EndTargetSelectionRequested msg)
    {
        _targeting = false;
        Message.Publish(new DirectionalInputNodeMapDisabled(_nodeMap));
}

    protected override void Execute(CancelTargetSelectionRequested msg)
    {
        _targeting = false;
        Message.Publish(new DirectionalInputNodeMapDisabled(_nodeMap));
    }
}