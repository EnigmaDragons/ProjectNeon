using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapNodeGameObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button button;
    [SerializeField] private StageSegment segment;
    [SerializeField] private GameObject hoverRulesPanel;
    [SerializeField] private Sprite hiddenNodeGraphic;
    [SerializeField] private CurrentGameMap2 gameMap;

    private string _nodeId;
    private SpriteState _revealedSpriteState;
    private SpriteState _hiddenSpriteState;
    private IStageSegment _arrivalSegment;
    private Maybe<GameObject> _rulesPanel;

    public void Init(string nodeId, bool canTravelTo)
    {
        _nodeId = nodeId;
        _arrivalSegment = segment;
        button.onClick.AddListener(() => Message.Publish(new TravelToNode { Node = gameObject, OnArrive = () => _arrivalSegment.Start(), NodeId = nodeId }));
        _revealedSpriteState = button.spriteState;
        _hiddenSpriteState = new SpriteState() { pressedSprite = hiddenNodeGraphic, disabledSprite = hiddenNodeGraphic, highlightedSprite = hiddenNodeGraphic, selectedSprite = hiddenNodeGraphic };
        SetCanTravelTo(canTravelTo);
    }

    private void Awake()
    {
        _rulesPanel = hoverRulesPanel != null ? new Maybe<GameObject>(hoverRulesPanel) : Maybe<GameObject>.Missing();
        _rulesPanel.IfPresent(r => r.SetActive(false));
    }

    public void ConvertToDeterministic(AdventureGenerationContext ctx) => _arrivalSegment = segment.GenerateDeterministic(ctx);

    public void SetCanTravelTo(bool canTravelTo)
    {
        button.interactable = canTravelTo;
        button.spriteState = canTravelTo || gameMap.CurrentMapNode.X + 20 >= gameMap.GetMapNode(_nodeId).X ? _revealedSpriteState : _hiddenSpriteState;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable)
            return;
        _arrivalSegment.Detail.IfPresent(detail => Message.Publish(new ShowTooltip(detail, true)));
        _rulesPanel.IfPresent(r => r.SetActive(true));
        transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!button.interactable)
            return;
        _rulesPanel.IfPresent(r => r.SetActive(false));
        Message.Publish(new HideTooltip());
    }
}