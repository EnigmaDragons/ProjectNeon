using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapNodeGameObject3 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button button;
    [SerializeField] private StageSegment segment;
    [SerializeField] private GameObject hoverRulesPanel;
    [SerializeField] private GameObject travelPreventedVisual;
    [SerializeField] private GameObject plotEventVisual;
    [SerializeField] private TextMeshProUGUI unvisitedTextLabel;
    [SerializeField] private TextMeshProUGUI visitedTextLabel;
    [SerializeField] private bool alwaysShowRules = false;
    [SerializeField] private GameObject[] imagesToEnlarge;
    [SerializeField] private FloatReference enlargeAmount;
    [SerializeField] private GameObject[] objectsToOffsetWhenEnlarged;
    [SerializeField] private FloatReference enlargedYOffsets;
    
    public IStageSegment ArrivalSegment { get; private set; }
    public MapNode3 MapData { get; private set; }
    private Maybe<GameObject> _rulesPanel;

    public void Init(MapNode3 mapData, CurrentGameMap3 gameMap, AdventureGenerationContext ctx, AllStaticGlobalEffects allGlobalEffects, Action<Transform> onMidPointArrive)
        => Init(mapData, gameMap, segment.GenerateDeterministic(ctx, mapData), allGlobalEffects, onMidPointArrive);
    
    public void Init(MapNode3 mapData, CurrentGameMap3 gameMap, IStageSegment stageSegment, AllStaticGlobalEffects allGlobalEffects, Action<Transform> onMidPointArrive)
    {
        MapData = mapData;
        ArrivalSegment = stageSegment;
        var canTravel = !mapData.PreventTravel;
        if (plotEventVisual != null)
            plotEventVisual.SetActive(mapData.IsPlotNode);
        if (travelPreventedVisual != null)
            travelPreventedVisual.SetActive(!canTravel);
        if (unvisitedTextLabel != null)
            unvisitedTextLabel.text = allGlobalEffects.GetEffectById(mapData.UnVisitedGlobalEffectId)
                .Select(e => $"<b>If Not Visited:</b>\n{e.FullDescription}", "");
        if (visitedTextLabel != null)
            visitedTextLabel.text = allGlobalEffects.GetEffectById(mapData.VisitedGlobalEffectId)
                .Select(e => $"<b>If Visited</b>:\n{e.FullDescription}", "");
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

    public void InitForV5(MapNode3 mapData, CurrentMapSegmentV5 gameMap, StageSegment stageSegment, AllStaticGlobalEffects allGlobalEffects, bool isEnlarged, Action<Transform> onMidPointArrive)
    {
        MapData = mapData;
        ArrivalSegment = stageSegment;
        var canTravel = !mapData.PreventTravel;
        if (plotEventVisual != null)
            plotEventVisual.SetActive(mapData.IsPlotNode);
        if (travelPreventedVisual != null)
            travelPreventedVisual.SetActive(!canTravel);
        if (unvisitedTextLabel != null)
            unvisitedTextLabel.text = allGlobalEffects.GetEffectById(mapData.UnVisitedGlobalEffectId)
                .Select(e => $"<b>If Not Visited:</b>\n{e.FullDescription}", "");
        if (visitedTextLabel != null)
            visitedTextLabel.text = allGlobalEffects.GetEffectById(mapData.VisitedGlobalEffectId)
                .Select(e => $"<b>If Visited</b>:\n{e.FullDescription}", "");
        button.enabled = canTravel;
        if (canTravel)
            button.onClick.AddListener(() =>
            {
                Action action = () =>
                {
                    gameMap.CurrentNode = mapData;
                    gameMap.CurrentChoices.Remove(mapData);
                    Message.Publish(new TravelToNode
                    {
                        Position = mapData.Position, 
                        OnMidPointArrive = onMidPointArrive,
                        OnArrive = t =>
                        {
                            Message.Publish(new TravelMovementStopped(t));
                            Message.Publish(new ArrivedAtNode(transform, mapData.Type));
                            if (mapData.AdvancesAdventure)
                                gameMap.AdvanceToNextSegment();
                            ArrivalSegment.Start();
                        }
                    });
                    Message.Publish(new AutoSaveRequested());
                };

                if (isEnlarged && !CurrentAcademyData.Data.ConfirmedStorySkipBehavior && mapData.Type != MapNodeType.MainStory && gameMap.CurrentChoices.Any(n => n.Type == MapNodeType.MainStory))
                {
                    Message.Publish(new ShowTwoChoiceDialog
                    {     
                        UseDarken = true,                   
                        Prompt = "You are skipping a Main Story segment. If you don't visit the Main Story segment right now, you will miss it for this entire run. Are you sure you wish to skip it?",
                        PrimaryButtonText = "Oops! Thanks!",
                        PrimaryAction = () => { },
                        SecondaryButtonText = "Skip the Story",
                        SecondaryAction = () =>
                        {
                            CurrentAcademyData.Mutate(d => d.ConfirmedStorySkipBehavior = true);
                            action();
                        }
                    });
                }
                else
                    action();
            });
        if (isEnlarged)
        {
            foreach (var image in imagesToEnlarge)
            {
                var scale = image.transform.localScale;
                image.transform.localScale = new Vector3(scale.x * enlargeAmount, scale.y * enlargeAmount, scale.z);
            }
            foreach (var obj in objectsToOffsetWhenEnlarged)
            {
                var pos = obj.transform.localPosition;
                obj.transform.localPosition = new Vector3(pos.x, pos.y + enlargedYOffsets, pos.z);
            }
        }
    }
    
    private void Awake()
    {
        _rulesPanel = hoverRulesPanel != null ? new Maybe<GameObject>(hoverRulesPanel) : Maybe<GameObject>.Missing();
        _rulesPanel.IfPresent(r => r.SetActive(alwaysShowRules));
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ArrivalSegment == null)
        {
            Log.Error("Map Node Game Object has no Arrival Segment");
            return;
        }
        
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