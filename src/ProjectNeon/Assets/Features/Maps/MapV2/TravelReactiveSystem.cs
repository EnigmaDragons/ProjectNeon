﻿using System;
using UnityEngine;

public class TravelReactiveSystem : OnMessage<TravelToNode, ContinueTraveling, FinishedStoryEvent>
{
    [SerializeField] private AdventureProgress2 adventure;
    [SerializeField] private CurrentGameMap3 gameMap;
    [SerializeField] private CurrentMapSegmentV5 mapV5;
    [SerializeField] private float speed;
    [HideInInspector] public GameObject PlayerToken { private get; set; }

    private bool _isTraveling;
    private Vector2 _midPoint;
    private Vector2 _travelTo;
    private Action<Transform> _onMidPointArrive;
    private Action<Transform> _onArrive;
    private bool _travelingToMidpoint;

    protected override void Execute(ContinueTraveling msg)
    {
        _isTraveling = true;
    }

    protected override void Execute(FinishedStoryEvent msg)
    {
        gameMap.HasCompletedEventEnRoute = true;
        gameMap.PreviousPosition = _midPoint;
        _isTraveling = !msg.ShouldNotContinue;
        Message.Publish(new TravelingSFX(transform));
    }

    protected override void Execute(TravelToNode msg)
    {
        if (_isTraveling)
            return;
        if (!msg.TravelInstantly)
            adventure.AdvanceStageIfNeeded();
        
        _isTraveling = true;
        _travelingToMidpoint = !gameMap.HasCompletedEventEnRoute;
        gameMap.PreviousPosition = gameMap.DestinationPosition;
        gameMap.DestinationPosition = msg.Position;
        mapV5.PreviousPosition = mapV5.DestinationPosition;
        mapV5.DestinationPosition = msg.Position;
        _travelTo = msg.Position;
        Message.Publish(new TravelingSFX(transform));
        _midPoint = (new Vector2(PlayerToken.transform.localPosition.x, PlayerToken.transform.localPosition.y) + _travelTo) / 2;
        _onMidPointArrive = msg.OnMidPointArrive;
        _onArrive = msg.OnArrive;
        PlayerToken.GetComponent<Floating>().enabled = false;
        if (msg.TravelInstantly)
            TravelInstantly();
    }

    private void TravelInstantly()
    {
        _isTraveling = false;
        PlayerToken.transform.localPosition = _travelTo;
        _onArrive(PlayerToken.transform);
        Log.Info($"Travel Instantly Finished");
    }
    
    private void Update()
    {
        if (!_isTraveling)
            return;

        if (Vector3.Distance(PlayerToken.transform.localPosition, _travelingToMidpoint ? _midPoint : _travelTo) < 0.01f)
        {
            if (_travelingToMidpoint)
            {
                _travelingToMidpoint = false;
                _isTraveling = false;
                _onMidPointArrive(PlayerToken.transform);
            }
            else
            {
                Log.Info("Finished Traveling", this);
                _onArrive(PlayerToken.transform);
                StartFloating();
                _isTraveling = false;
                gameMap.HasCompletedEventEnRoute = false;
            }
        }

        PlayerToken.transform.localPosition = Vector3.MoveTowards(PlayerToken.transform.localPosition, _travelingToMidpoint ? _midPoint : _travelTo, speed * Time.deltaTime);
    }
    
    private void StartFloating()
    {
        var floating = PlayerToken.GetComponent<Floating>();
        if (floating != null)
            floating.enabled = true;
    }
}