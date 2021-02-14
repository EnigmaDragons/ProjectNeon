using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TweenUIMovement : OnMessage<TweenMovementRequested, StopMovementTweeningRequested>
{
    private TweenProgress[] _progresses = new TweenProgress[0];

    public void Update()
    {
        foreach (var progress in _progresses.Where(x => x.T < 1))
        {
            float beforeT = progress.T;
            progress.T = Math.Min(1, beforeT + Time.deltaTime / progress.Seconds);
            float afterT = progress.T;
            var beforeDistance = progress.RelativeDistance * beforeT; //EaseInOutCubic(beforeT);
            var afterDistance = progress.RelativeDistance * afterT; //EaseInOutCubic(afterT);
            var currentDistance = afterDistance - beforeDistance;
            progress.Transform.position += currentDistance;
        }
    }

    protected override void Execute(TweenMovementRequested msg)
        => _progresses = _progresses
            .Where(x => x.T < 1)
            .Concat(new[] { new TweenProgress { Transform = msg.Transform, RelativeDistance = msg.RelativeDistance, Seconds = msg.Seconds } })
            .ToArray();

    protected override void Execute(StopMovementTweeningRequested msg)
        => _progresses = _progresses.Where(x => x.T < 1 && x.Transform != msg.Transform).ToArray();

    private float EaseInOutCubic(float progress) => progress < 0.5f ? 4f * progress * progress * progress : 1f - Mathf.Pow(-2f * progress + 2f, 3f) / 2f;
    
    private class TweenProgress
    {
        public Transform Transform;
        public Vector3 RelativeDistance;
        public float Seconds;
        public float T;
    }
}