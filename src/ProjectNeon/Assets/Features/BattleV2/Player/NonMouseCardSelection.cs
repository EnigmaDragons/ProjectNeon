using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NonMouseCardSelection : MonoBehaviour
{
    [SerializeField] private BattleState state;
    [SerializeField] private HandVisualizerBase hand;
    [SerializeField] private Button cycleOrDiscard;
    [SerializeField] private Button endTurn;

    private DirectionalInputNodeMap _nodeMap;
    private bool _shouldDiscard;
    private CardPresenter _cardPresenter;
    
    private void Awake()
    {
        cycleOrDiscard.onClick.AddListener(() =>
        {
            var cardComponent = hand.ShownCards.First(x => x.IsSelected && x.gameObject.activeInHierarchy);
            if (state.NumberOfRecyclesRemainingThisTurn > 0)
                cardComponent.MiddleClick();
            else
            {
                _cardPresenter = cardComponent;
                _cardPresenter.IsDragging = true;
                _shouldDiscard = true;
                Message.Publish(new BeginTargetSelectionRequested(cardComponent.Card, cardComponent));
            }
        });
    }

    private void OnEnable()
    {
        Message.Subscribe<CardPositionsUpdated>(_ => UpdateNodeMap(), this);
        Message.Subscribe<TargetSelectionBegun>(_ => DiscardIfNeeded(), this);
        _nodeMap = GenerateNewNodeMap();
        Message.Publish(new DirectionalInputNodeMapDisabled(_nodeMap));
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
        Message.Publish(new DirectionalInputNodeMapDisabled(_nodeMap));
    }

    private void UpdateNodeMap()
    {
        var newMap = GenerateNewNodeMap();
        Message.Publish(new DirectionalInputNodeMapChanged(_nodeMap, newMap));
        _nodeMap = newMap;
    }

    private DirectionalInputNodeMap GenerateNewNodeMap()
    {
        var nodes = hand.ShownCards.Where(x => x.gameObject.activeInHierarchy).OrderBy(x => x.Position.x).Select(x => new DirectionalInputNode { Selectable = x.gameObject }).ToArray();
        for (var i = 0; i < nodes.Length; i++)
        {
            if (i != 0)
                nodes[i].Left = nodes[i - 1].Selectable;
            if (i != nodes.Length - 1)
                nodes[i].Right = nodes[i + 1].Selectable;
        }
        return new DirectionalInputNodeMap
        {
            Z = 0,
            BackObject = cycleOrDiscard.gameObject,
            NextObject = endTurn.gameObject,
            PreviousObject = endTurn.gameObject,
            DefaultSelected = nodes.Select(x => x.Selectable).ToArray(),
            Nodes = nodes
        };
    }

    private void DiscardIfNeeded()
    {
        if (_shouldDiscard)
        {
            _shouldDiscard = false;
            _cardPresenter.IsDragging = false;
            _cardPresenter.Discard();
            Message.Publish(new CheckForAutomaticTurnEnd());
        }
    }
}