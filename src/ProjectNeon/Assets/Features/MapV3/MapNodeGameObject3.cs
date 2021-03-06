﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapNodeGameObject3 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button button;
    [SerializeField] private StageSegment segment;
    [SerializeField] private GameObject hoverRulesPanel;
    
    private IStageSegment _arrivalSegment;
    private Maybe<GameObject> _rulesPanel;
    
    public void Init(MapNode3 mapData, AdventureGenerationContext ctx, Action onArrive, Action onMidPointArrive)
    {
        _arrivalSegment = segment;
        button.onClick.AddListener(() => Message.Publish(new TravelToNode { Position = mapData.Position, OnMidPointArrive = onMidPointArrive, OnArrive = () =>
            {
                onArrive();
                _arrivalSegment.Start();
            },
        }));
        _arrivalSegment = _arrivalSegment.GenerateDeterministic(ctx, mapData);
    }
    
    private void Awake()
    {
        _rulesPanel = hoverRulesPanel != null ? new Maybe<GameObject>(hoverRulesPanel) : Maybe<GameObject>.Missing();
        _rulesPanel.IfPresent(r => r.SetActive(false));
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        _arrivalSegment.Detail.IfPresent(detail => Message.Publish(new ShowTooltip(detail, true)));
        _rulesPanel.IfPresent(r => r.SetActive(true));
        transform.SetSiblingIndex(transform.parent.childCount - 2);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _rulesPanel.IfPresent(r => r.SetActive(false));
        Message.Publish(new HideTooltip());
    }
}