using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapNodeGameObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button button;
    [SerializeField] private StageSegment segment;
    [SerializeField] private GameObject hoverRulesPanel;

    private IStageSegment _arrivalSegment;
    private Maybe<GameObject> _rulesPanel;
    
    public void Init(string nodeId, bool canTravelTo)
    {
        _arrivalSegment = segment;
        button.interactable = canTravelTo;
        button.onClick.AddListener(() => Message.Publish(new TravelToNode { Node = gameObject, OnArrive = () => _arrivalSegment.Start(), NodeId = nodeId }));
    }

    private void Awake()
    {
        _rulesPanel = hoverRulesPanel != null ? new Maybe<GameObject>(hoverRulesPanel) : Maybe<GameObject>.Missing();
        _rulesPanel.IfPresent(r => r.SetActive(false));
    }

    public void ConvertToDeterministic(AdventureGenerationContext ctx) => _arrivalSegment = segment.GenerateDeterministic(ctx);
    public void SetCanTravelTo(bool canTravelTo) => button.interactable = canTravelTo;
    public void OnPointerEnter(PointerEventData eventData)
    {
        _arrivalSegment.Detail.IfPresent(detail => Message.Publish(new ShowTooltip(detail, true)));
        _rulesPanel.IfPresent(r => r.SetActive(true));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _rulesPanel.IfPresent(r => r.SetActive(false));
        Message.Publish(new HideTooltip());
    }
}