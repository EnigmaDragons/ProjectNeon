using System;
using System.Linq;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapNodeGameObject3 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ILocalizeTerms
{
    [SerializeField] private Button button;
    [SerializeField] private StageSegment segment;
    [SerializeField] private GameObject travelPreventedVisual;
    [SerializeField] private GameObject plotEventVisual;
    [SerializeField] private Localize unvisitedTextLabel;
    [SerializeField] private Localize visitedTextLabel;
    
    [Header("Enlargement")]
    [SerializeField] private GameObject[] imagesToEnlarge;
    [SerializeField] private FloatReference enlargeAmount;
    [SerializeField] private GameObject[] objectsToOffsetWhenEnlarged;
    [SerializeField] private FloatReference enlargedYOffsets;
    
    [Header("Hover Description")]
    [SerializeField] private bool alwaysShowRules = false;
    [SerializeField] private MapNodeDescription description;
    [SerializeField] private string nodeTerm;
    
    public IStageSegment ArrivalSegment { get; private set; }
    public MapNode3 MapData { get; private set; }
    private Maybe<GameObject> _rulesPanel;

    private string NodeName => $"Maps/{nodeTerm}_Name";
    private string NodeDetail => $"Maps/{nodeTerm}_Detail";
    private const string NotVisitedEffect = "Maps/NotVisitedEffect";
    private const string VisitedEffect = "Maps/VisitedEffect";
    
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
            unvisitedTextLabel.SetFinalText(allGlobalEffects.GetEffectById(mapData.UnVisitedGlobalEffectId)
                .Select(e => NotVisitedEffect.ToLocalized().SafeFormatWithDefault("<b>If Not Visited:</b>\n{0}", e.FullDescriptionTerm.ToLocalized()), ""));
        if (visitedTextLabel != null)
            visitedTextLabel.SetFinalText(allGlobalEffects.GetEffectById(mapData.VisitedGlobalEffectId)
                .Select(e => VisitedEffect.ToLocalized().SafeFormatWithDefault("<b>If Visited:</b>\n{0}", e.FullDescriptionTerm.ToLocalized()), ""));
        if (description != null)
            description.Init(NodeName, NodeDetail);
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

    public void InitForV5(MapNode3 mapData, CurrentMapSegmentV5 gameMap, StageSegment stageSegment, AllStaticGlobalEffects allGlobalEffects, bool isEnlarged, Action<Transform> onMidPointArrive, BoolVariable skippingStory)
    {
        MapData = mapData;
        ArrivalSegment = stageSegment;
        var canTravel = !mapData.PreventTravel;
        if (plotEventVisual != null)
            plotEventVisual.SetActive(mapData.IsPlotNode);
        if (travelPreventedVisual != null)
            travelPreventedVisual.SetActive(!canTravel);
        if (unvisitedTextLabel != null)
            unvisitedTextLabel.SetFinalText(allGlobalEffects.GetEffectById(mapData.UnVisitedGlobalEffectId)
                .Select(e => NotVisitedEffect.ToLocalized().SafeFormatWithDefault("<b>If Not Visited:</b>\n{0}", e.FullDescriptionTerm.ToLocalized()), ""));
        if (visitedTextLabel != null)
            visitedTextLabel.SetFinalText(allGlobalEffects.GetEffectById(mapData.VisitedGlobalEffectId)
                .Select(e => VisitedEffect.ToLocalized().SafeFormatWithDefault("<b>If Visited:</b>\n{0}", e.FullDescriptionTerm.ToLocalized()), ""));
        if (description != null)
            description.Init(NodeName, NodeDetail, stageSegment);
        button.enabled = canTravel;
        if (canTravel)
            button.onClick.AddListener(() =>
            {
                Action action = () =>
                {
                    if (mapData.Type != MapNodeType.MainStory && gameMap.CurrentChoices.Any(n => n.Type == MapNodeType.MainStory))
                        skippingStory.SetValue(true);
                    else if (mapData.Type == MapNodeType.MainStory)
                        skippingStory.SetValue(false);
                    gameMap.CurrentNode = mapData;
                    gameMap.CurrentChoices.Remove(mapData);
                    gameMap.IncludeCurrentNodeInSaveData = true;
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
                    Message.Publish(new ShowLocalizedDialog
                    {
                        UseDarken = true,
                        PromptTerm = DialogTerms.SkipStoryWarning,
                        PrimaryButtonTerm = DialogTerms.OptionOops,
                        PrimaryAction = () => { },
                        SecondaryButtonTerm = DialogTerms.OptionSkipTheStory,
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
        _rulesPanel = description != null ? new Maybe<GameObject>(description.gameObject) : Maybe<GameObject>.Missing();
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
        
        ArrivalSegment.Detail.IfPresent(detail => Message.Publish(new ShowTooltip(transform.position, detail, true)));
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

    public string[] GetLocalizeTerms() => new []
    {
        DialogTerms.OptionOops, 
        DialogTerms.SkipStoryWarning, 
        DialogTerms.OptionSkipTheStory,
        NodeName, 
        NodeDetail,
        NotVisitedEffect,
        VisitedEffect
    };
}