using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapNodeGameObject3 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject travelPreventedVisual;
    [SerializeField] private Button button;
    [SerializeField] private StageSegment segment;
    [SerializeField] private GameObject hoverRulesPanel;
    [SerializeField] private bool alwaysShowRules = false;
    
    public IStageSegment ArrivalSegment { get; private set; }
    public MapNode3 MapData { get; private set; }
    private Maybe<GameObject> _rulesPanel;

    public void Init(MapNode3 mapData, CurrentGameMap3 gameMap, AdventureGenerationContext ctx, Action<Transform> onMidPointArrive)
    {
        MapData = mapData;
        ArrivalSegment = segment.GenerateDeterministic(ctx, mapData);
        var canTravel = !mapData.PreventTravel;
        if (travelPreventedVisual != null)
            travelPreventedVisual.SetActive(!canTravel);
        button.enabled = canTravel;
        if (canTravel)
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
                        OnArrive = t =>
                        {
                            Message.Publish(new TravelMovementStopped(t));
                            Message.Publish(new ArrivedAtNode(transform, mapData.Type));
                            ArrivalSegment.Start();
                        }
                    });
                Message.Publish(new AutoSaveRequested());
            });
    }
    
    private void Awake()
    {
        _rulesPanel = hoverRulesPanel != null ? new Maybe<GameObject>(hoverRulesPanel) : Maybe<GameObject>.Missing();
        _rulesPanel.IfPresent(r => r.SetActive(alwaysShowRules));
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (alwaysShowRules)
            return;
        
        ArrivalSegment.Detail.IfPresent(detail => Message.Publish(new ShowTooltip(transform, detail, true)));
        _rulesPanel.IfPresent(r => r.SetActive(true));
        transform.SetSiblingIndex(transform.parent.childCount - 2);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (alwaysShowRules)
            return;
        
        _rulesPanel.IfPresent(r => r.SetActive(false));
        Message.Publish(new HideTooltip());
    }
}