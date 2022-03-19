using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace MoreMountains.Feedbacks
{
    [RequireComponent(typeof(MMFeedbacks))]
    [AddComponentMenu("More Mountains/Feedbacks/Shakers/Feedbacks/MMFeedbacksShaker")]
    public class MMFeedbacksShaker : MMShaker
    {
        protected MMFeedbacks _mmFeedbacks;

        /// <summary>
        /// On init we initialize our values
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            _mmFeedbacks = this.gameObject.GetComponent<MMFeedbacks>();
        }

        public virtual void OnMMFeedbacksShakeEvent(int channel = 0, bool useRange = false, float eventRange = 0f, Vector3 eventOriginPosition = default(Vector3))
        {
            if (!CheckEventAllowed(channel, useRange, eventRange, eventOriginPosition) || (!Interruptible && Shaking))
            {
                return;
            }
            Play();
        }

        protected override void ShakeStarts()
        {
            _mmFeedbacks.PlayFeedbacks();
        }

        /// <summary>
        /// When that shaker gets added, we initialize its shake duration
        /// </summary>
        protected virtual void Reset()
        {
            ShakeDuration = 0.01f;
        }

        /// <summary>
        /// Starts listening for events
        /// </summary>
        public override void StartListening()
        {
            base.StartListening();
            MMFeedbacksShakeEvent.Register(OnMMFeedbacksShakeEvent);
        }

        /// <summary>
        /// Stops listening for events
        /// </summary>
        public override void StopListening()
        {
            base.StopListening();
            MMFeedbacksShakeEvent.Unregister(OnMMFeedbacksShakeEvent);
        }
    }

    public struct MMFeedbacksShakeEvent
    {
        public delegate void Delegate(int channel = 0, bool useRange = false, float eventRange = 0f, Vector3 eventOriginPosition = default(Vector3));
        static private event Delegate OnEvent;

        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        static public void Trigger(int channel = 0, bool useRange = false, float eventRange = 0f, Vector3 eventOriginPosition = default(Vector3))
        {
            OnEvent?.Invoke(channel, useRange, eventRange, eventOriginPosition);
        }
    }
}
