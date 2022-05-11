using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// Add this class to a Camera with a HDRP vignette post processing and it'll be able to "shake" its values by getting events
    /// </summary>
    [RequireComponent(typeof(Volume))]
    [AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMPaniniProjectionShaker_HDRP")]
    public class MMPaniniProjectionShaker_HDRP : MMShaker
    {
        [Header("Distance")]
        /// whether or not to add to the initial value
        [Tooltip("whether or not to add to the initial value")]
        public bool RelativeDistance = false;
        /// the curve used to animate the distance value on
        [Tooltip("the curve used to animate the distance value on")]
        public AnimationCurve ShakeDistance = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        /// the value to remap the curve's 0 to
        [Tooltip("the value to remap the curve's 0 to")]
        [Range(0f, 1f)]
        public float RemapDistanceZero = 0f;
        /// the value to remap the curve's 1 to
        [Tooltip("the value to remap the curve's 1 to")]
        [Range(0f, 1f)]
        public float RemapDistanceOne = 1f;

        protected Volume _volume;
        protected PaniniProjection _paniniProjection;
        protected float _initialDistance;
        protected float _originalShakeDuration;
        protected AnimationCurve _originalShakeDistance;
        protected float _originalRemapDistanceZero;
        protected float _originalRemapDistanceOne;
        protected bool _originalRelativeDistance;

        /// <summary>
        /// On init we initialize our values
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            _volume = this.gameObject.GetComponent<Volume>();
            _volume.profile.TryGet(out _paniniProjection);
        }

        /// <summary>
        /// Shakes values over time
        /// </summary>
        protected override void Shake()
        {
            float newValue = ShakeFloat(ShakeDistance, RemapDistanceZero, RemapDistanceOne, RelativeDistance, _initialDistance);
            _paniniProjection.distance.Override(newValue);
        }

        /// <summary>
        /// Collects initial values on the target
        /// </summary>
        protected override void GrabInitialValues()
        {
            _initialDistance = _paniniProjection.distance.value;
        }

        /// <summary>
        /// When we get the appropriate event, we trigger a shake
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="duration"></param>
        /// <param name="amplitude"></param>
        /// <param name="relativeDistance"></param>
        /// <param name="attenuation"></param>
        /// <param name="channel"></param>
        public virtual void OnPaniniProjectionShakeEvent(AnimationCurve distance, float duration, float remapMin, float remapMax, bool relativeDistance = false,
            float attenuation = 1.0f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
        {
            if (!CheckEventAllowed(channel) || (!Interruptible && Shaking))
            {
                return;
            }
            
            if (stop)
            {
                Stop();
                return;
            }
            
            _resetShakerValuesAfterShake = resetShakerValuesAfterShake;
            _resetTargetValuesAfterShake = resetTargetValuesAfterShake;

            if (resetShakerValuesAfterShake)
            {
                _originalShakeDuration = ShakeDuration;
                _originalShakeDistance = ShakeDistance;
                _originalRemapDistanceZero = RemapDistanceZero;
                _originalRemapDistanceOne = RemapDistanceOne;
                _originalRelativeDistance = RelativeDistance;
            }

            TimescaleMode = timescaleMode;
            ShakeDuration = duration;
            ShakeDistance = distance;
            RemapDistanceZero = remapMin * attenuation;
            RemapDistanceOne = remapMax * attenuation;
            RelativeDistance = relativeDistance;
            ForwardDirection = forwardDirection;

            Play();
        }

        /// <summary>
        /// Resets the target's values
        /// </summary>
        protected override void ResetTargetValues()
        {
            base.ResetTargetValues();
            _paniniProjection.distance.Override(_initialDistance);
        }

        /// <summary>
        /// Resets the shaker's values
        /// </summary>
        protected override void ResetShakerValues()
        {
            base.ResetShakerValues();
            ShakeDuration = _originalShakeDuration;
            ShakeDistance = _originalShakeDistance;
            RemapDistanceZero = _originalRemapDistanceZero;
            RemapDistanceOne = _originalRemapDistanceOne;
            RelativeDistance = _originalRelativeDistance;
        }

        /// <summary>
        /// Starts listening for events
        /// </summary>
        public override void StartListening()
        {
            base.StartListening();
            MMPaniniProjectionShakeEvent_HDRP.Register(OnPaniniProjectionShakeEvent);
        }

        /// <summary>
        /// Stops listening for events
        /// </summary>
        public override void StopListening()
        {
            base.StopListening();
            MMPaniniProjectionShakeEvent_HDRP.Unregister(OnPaniniProjectionShakeEvent);
        }
    }

    /// <summary>
    /// An event used to trigger vignette shakes
    /// </summary>
    public struct MMPaniniProjectionShakeEvent_HDRP
    {
        public delegate void Delegate(AnimationCurve distance, float duration, float remapMin, float remapMax, bool relativeDistance = false,
            float attenuation = 1.0f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false);
        static private event Delegate OnEvent;
        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }
        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }
        static public void Trigger(AnimationCurve distance, float duration, float remapMin, float remapMax, bool relativeDistance = false,
            float attenuation = 1.0f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
        {
            OnEvent?.Invoke(distance, duration, remapMin, remapMax, relativeDistance, attenuation, channel, resetShakerValuesAfterShake, resetTargetValuesAfterShake, forwardDirection, timescaleMode, stop);
        }
    }
}
