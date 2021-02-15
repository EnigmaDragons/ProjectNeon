using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class TweenUIMovement : OnMessage<TweenMovementRequested, StopMovementTweeningRequested, SnapBackTweenRequested, GoToTweenRequested>
{
    private ConcurrentQueue<QueuedTweenRequest> _inbox = new ConcurrentQueue<QueuedTweenRequest>();
    private List<TweenProgress> _movements = new List<TweenProgress>();

    protected override void Execute(TweenMovementRequested msg) => _inbox.Enqueue(new QueuedTweenRequest(msg));
    protected override void Execute(SnapBackTweenRequested msg) => _inbox.Enqueue(new QueuedTweenRequest(msg));
    protected override void Execute(GoToTweenRequested msg) => _inbox.Enqueue(new QueuedTweenRequest(msg));
    protected override void Execute(StopMovementTweeningRequested msg) => _inbox.Enqueue(new QueuedTweenRequest(msg));

    private void Update()
    {
        CleanUpFinishedMovements();
        ProcessInbox();
        ProcessMovements();
    }

    private void CleanUpFinishedMovements()
    {
        foreach (var movement in _movements.ToArray())
            if ((movement.MovementType != TweenMovementType.RubberBand && movement.T >= 1) || (movement.Reverse && movement.T <= 0))
                _movements.Remove(movement);
    }

    private void ProcessInbox()
    {
        while (_inbox.TryDequeue(out var request))
        {
            if (request.RequestType == TweenRequestType.Move)
                ProcessMovementRequest(request.MoveRequest);
            else if (request.RequestType == TweenRequestType.SnapBack)
                ProcessSnapBackRequest(request.SnapBackRequest);
            else if (request.RequestType == TweenRequestType.GoTo)
                ProcessGoToRequest(request.GoToRequest);
            else if (request.RequestType == TweenRequestType.Stop)
                ProcessStopRequest(request.StopRequest);
        }
    }

    private void ProcessMovementRequest(TweenMovementRequested request)
    {
        var shouldAdd = true;
        if (!string.IsNullOrWhiteSpace(request.MovementName))
        {
            foreach (var current in _movements.ToArray())
            {
                if (current.MovementName == request.MovementName && current.Transform == request.Transform)
                {
                    shouldAdd = false;
                    if (current.Reverse)
                        current.Reverse = false;
                }
            }   
        }
        if (shouldAdd)
            _movements.Add(new TweenProgress
            {
                Transform = request.Transform, 
                RelativeDistance = request.RelativeDistance, 
                Seconds = request.Seconds, 
                MovementType = request.MovementType, 
                MovementName = request.MovementName,
                Dimension = request.Dimension
            });
    }

    private void ProcessSnapBackRequest(SnapBackTweenRequested request)
    {
        foreach (var movement in _movements.ToArray())
            if (movement.MovementName == request.MovementName && movement.Transform == request.Transform && movement.MovementType == TweenMovementType.RubberBand)
                movement.Reverse = true;
    }

    private void ProcessGoToRequest(GoToTweenRequested request)
    {
        var currentDestination = Vector3.zero;
        if (request.Dimension == MovementDimension.Spatial)
            currentDestination = request.Transform.position;
        else if (request.Dimension == MovementDimension.Scale)
            currentDestination = request.Transform.localScale;
        foreach (var movement in _movements)
        {
            if (movement.Transform == request.Transform && movement.Dimension == request.Dimension)
            {
                if (movement.MovementType != TweenMovementType.RubberBand)
                {
                    var beforeDistance = movement.RelativeDistance * EaseInOutCubic(movement.T);
                    var endDistance = movement.RelativeDistance * 1;
                    var difference = endDistance - beforeDistance;
                    currentDestination += difference;   
                }
                else
                {
                    var beforeDistance = movement.RelativeDistance * EaseInOutCubic(movement.T);
                    currentDestination -= beforeDistance;   
                }
            }
        }
        _movements.Add(new TweenProgress
        {
            Transform = request.Transform,
            RelativeDistance = request.Destination - currentDestination,
            Seconds = request.Seconds,
            MovementType = TweenMovementType.GoTo,
            Dimension = request.Dimension
        });
    }

    private void ProcessStopRequest(StopMovementTweeningRequested request)
    {
        foreach (var movement in _movements.ToArray())
            if (movement.Transform == request.Transform && movement.Dimension == request.Dimension)
                _movements.Remove(movement);
    }

    private void ProcessMovements()
    {
        foreach (var movement in _movements)
        {
            var beforeT = movement.T;
            if (movement.Reverse)
                movement.T = Math.Max(0, beforeT - Time.deltaTime / movement.Seconds);
            else
                movement.T = Math.Min(1, beforeT + Time.deltaTime / movement.Seconds);
            var afterT = movement.T;
            var beforeDistance = movement.RelativeDistance * EaseInOutCubic(beforeT);
            var afterDistance = movement.RelativeDistance * EaseInOutCubic(afterT);
            var currentDistance = afterDistance - beforeDistance;
            if (movement.Dimension == MovementDimension.Spatial)
                movement.Transform.position += currentDistance;
            else if (movement.Dimension == MovementDimension.Scale)
                movement.Transform.localScale += currentDistance;
        }
    }

    private float EaseInOutCubic(float progress) => progress < 0.5f ? 4f * progress * progress * progress : 1f - Mathf.Pow(-2f * progress + 2f, 3f) / 2f;
    
    private class TweenProgress
    {
        public Transform Transform;
        public Vector3 RelativeDistance;
        public float Seconds;
        public TweenMovementType MovementType;
        public string MovementName;
        public MovementDimension Dimension;
        
        public float T;
        public bool Reverse;
    }

    private class QueuedTweenRequest
    {
        public TweenRequestType RequestType { get; }
        public TweenMovementRequested MoveRequest { get; }
        public SnapBackTweenRequested SnapBackRequest { get; }
        public GoToTweenRequested GoToRequest { get; }
        public StopMovementTweeningRequested StopRequest { get; }

        public QueuedTweenRequest(TweenMovementRequested request)
        {
            RequestType = TweenRequestType.Move;
            MoveRequest = request;
        }
        public QueuedTweenRequest(SnapBackTweenRequested request)
        {
            RequestType = TweenRequestType.SnapBack;
            SnapBackRequest = request;
        }
        public QueuedTweenRequest(GoToTweenRequested request)
        {
            RequestType = TweenRequestType.GoTo;
            GoToRequest = request;
        }
        public QueuedTweenRequest(StopMovementTweeningRequested request)
        {
            RequestType = TweenRequestType.Stop;
            StopRequest = request;
        }
    }

    private enum TweenRequestType
    {
        Move,
        SnapBack,
        GoTo,
        Stop
    }
}