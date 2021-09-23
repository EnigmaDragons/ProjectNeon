using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapNodeGameObject3 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button button;
    [SerializeField] private StageSegment segment;
    [SerializeField] private GameObject hoverRulesPanel;
    
    public IStageSegment ArrivalSegment { get; private set; }
    public MapNode3 MapData { get; private set; }
    private Maybe<GameObject> _rulesPanel;

    public void Init(MapNode3 mapData, CurrentGameMap3 gameMap, AdventureGenerationContext ctx, Action onMidPointArrive)
    {
        MapData = mapData;
        ArrivalSegment = segment;
        button.onClick.AddListener(() =>
        {
            gameMap.CurrentNode = mapData;
            AllMetrics.PublishMapNodeSelection(gameMap.Progress, 
                mapData.GetMetricDescription(), 
                gameMap.CurrentChoices.Select(m => m.GetMetricDescription()).ToArray());
            Message.Publish(new TravelToNode
                {
                    Position = mapData.Position, 
                    OnMidPointArrive = onMidPointArrive,
                    OnArrive = () =>
                    {
                        Message.Publish(new ArrivedAtNode(mapData.Type));
                        ArrivalSegment.Start();
                    }
                });
            Message.Publish(new AutoSaveRequested());
        });
        ArrivalSegment = ArrivalSegment.GenerateDeterministic(ctx, mapData);
    }
    
    private void Awake()
    {
        _rulesPanel = hoverRulesPanel != null ? new Maybe<GameObject>(hoverRulesPanel) : Maybe<GameObject>.Missing();
        _rulesPanel.IfPresent(r => r.SetActive(false));
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        ArrivalSegment.Detail.IfPresent(detail => Message.Publish(new ShowTooltip(detail, true)));
        _rulesPanel.IfPresent(r => r.SetActive(true));
        transform.SetSiblingIndex(transform.parent.childCount - 2);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _rulesPanel.IfPresent(r => r.SetActive(false));
        Message.Publish(new HideTooltip());
    }
}