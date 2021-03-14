using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapNodeGameObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button button;
    [SerializeField] private StageSegment segment;

    private IStageSegment _arrivalSegment;
    
    public void Init(string nodeId, bool canTravelTo)
    {
        _arrivalSegment = segment;
        button.interactable = canTravelTo;
        button.onClick.AddListener(() => Message.Publish(new TravelToNode { Node = gameObject, OnArrive = () => _arrivalSegment.Start(), NodeId = nodeId }));
    }

    public void ConvertToDeterministic(AdventureGenerationContext ctx) => _arrivalSegment = segment.GenerateDeterministic(ctx);
    public void SetCanTravelTo(bool canTravelTo) => button.interactable = canTravelTo;
    public void OnPointerEnter(PointerEventData eventData) => _arrivalSegment.Detail.IfPresent(detail => Message.Publish(new ShowTooltip(detail, true)));
    public void OnPointerExit(PointerEventData eventData) => Message.Publish(new HideTooltip());
}