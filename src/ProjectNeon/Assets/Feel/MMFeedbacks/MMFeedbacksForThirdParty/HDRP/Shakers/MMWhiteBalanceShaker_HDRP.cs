﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// Add this class to a Camera with a white balance post processing and it'll be able to "shake" its values by getting events
    /// </summary>
    [RequireComponent(typeof(Volume))]
    [AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMWhiteBalanceShaker_HDRP")]
    public class MMWhiteBalanceShaker_HDRP : MMShaker
    {
        /// whether or not to add to the initial value
        [Tooltip("whether or not to add to the initial value")]
        public bool RelativeValues = true;

        [Header("Temperature")]
        /// the curve used to animate the temperature value on
        [Tooltip("the curve used to animate the temperature value on")]
        public AnimationCurve ShakeTemperature = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        /// the value to remap the curve's 0 to
        [Tooltip("the value to remap the curve's 0 to")]
        [Range(-100f, 100f)]
        public float RemapTemperatureZero = 0f;
        /// the value to remap the curve's 1 to
        [Tooltip("the value to remap the curve's 1 to")]
        [Range(-100f, 100f)]
        public float RemapTemperatureOne = 100f;

        [Header("Tint")]
        /// the curve used to animate the tint value on
        [Tooltip("the curve used to animate the tint value on")]
        public AnimationCurve ShakeTint = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        /// the value to remap the curve's 0 to
        [Tooltip("the value to remap the curve's 0 to")]
        [Range(-100f, 100f)]
        public float RemapTintZero = 0f;
        /// the value to remap the curve's 1 to
        [Tooltip("the value to remap the curve's 1 to")]
        [Range(-100f, 100f)]
        public float RemapTintOne = 100f;

        protected Volume _volume;
        protected WhiteBalance _whiteBalance;
        protected float _initialTemperature;
        protected float _initialTint;
        protected float _originalShakeDuration;
        protected bool _originalRelativeValues;
        protected AnimationCurve _originalShakeTemperature;
        protected float _originalRemapTemperatureZero;
        protected float _originalRemapTemperatureOne;
        protected AnimationCurve _originalShakeTint;
        protected float _originalRemapTintZero;
        protected float _originalRemapTintOne;

        /// <summary>
        /// On init we initialize our values
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            _volume = this.gameObject.GetComponent<Volume>();
            _volume.profile.TryGet(out _whiteBalance);
        }

        /// <summary>
        /// Shakes values over time
        /// </summary>
        protected override void Shake()
        {
            float newTemperature = ShakeFloat(ShakeTemperature, RemapTemperatureZero, RemapTemperatureOne, RelativeValues, _initialTemperature);
            _whiteBalance.temperature.Override(newTemperature);
            float newTint = ShakeFloat(ShakeTint, RemapTintZero, RemapTintOne, RelativeValues, _initialTint);
            _whiteBalance.tint.Override(newTint);
        }

        /// <summary>
        /// Collects initial values on the target
        /// </summary>
        protected override void GrabInitialValues()
        {
            _initialTemperature = _whiteBalance.temperature.value;
            _initialTint = _whiteBalance.tint.value;
        }

        /// <summary>
        /// When we get the appropriate event, we trigger a shake
        /// </summary>
        /// <param name="temperature"></param>
        /// <param name="duration"></param>
        /// <param name="amplitude"></param>
        /// <param name="relativeValues"></param>
        /// <param name="attenuation"></param>
        /// <param name="channel"></param>
        public virtual void OnWhiteBalanceShakeEvent(AnimationCurve temperature, float duration, float remapTemperatureMin, float remapTemperatureMax,
            AnimationCurve tint, float remapTintMin, float remapTintMax, bool relativeValues = false,
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
                _originalShakeTemperature = ShakeTemperature;
                _originalRemapTemperatureZero = RemapTemperatureZero;
                _originalRemapTemperatureOne = RemapTemperatureOne;
                _originalRelativeValues = RelativeValues;
                _originalShakeTint = ShakeTint;
                _originalRemapTintZero = RemapTintZero;
                _originalRemapTintOne = RemapTintOne;
            }

            TimescaleMode = timescaleMode;
            ShakeDuration = duration;
            ShakeTemperature = temperature;
            RemapTemperatureZero = remapTemperatureMin * attenuation;
            RemapTemperatureOne = remapTemperatureMax * attenuation;
            RelativeValues = relativeValues;
            ShakeTint = tint;
            RemapTintZero = remapTintMin;
            RemapTintOne = remapTintMax;
            ForwardDirection = forwardDirection;

            Play();
        }

        /// <summary>
        /// Resets the target's values
        /// </summary>
        protected override void ResetTargetValues()
        {
            base.ResetTargetValues();
            _whiteBalance.temperature.Override(_initialTemperature);
            _whiteBalance.tint.Override(_initialTint);
        }

        /// <summary>
        /// Resets the shaker's values
        /// </summary>
        protected override void ResetShakerValues()
        {
            base.ResetShakerValues();
            ShakeDuration = _originalShakeDuration;
            ShakeTemperature = _originalShakeTemperature;
            RemapTemperatureZero = _originalRemapTemperatureZero;
            RemapTemperatureOne = _originalRemapTemperatureOne;
            RelativeValues = _originalRelativeValues;
            ShakeTint = _originalShakeTint;
            RemapTintZero = _originalRemapTintZero;
            RemapTintOne = _originalRemapTintOne;
        }

        /// <summary>
        /// Starts listening for events
        /// </summary>
        public override void StartListening()
        {
            base.StartListening();
            MMWhiteBalanceShakeEvent_HDRP.Register(OnWhiteBalanceShakeEvent);
        }

        /// <summary>
        /// Stops listening for events
        /// </summary>
        public override void StopListening()
        {
            base.StopListening();
            MMWhiteBalanceShakeEvent_HDRP.Unregister(OnWhiteBalanceShakeEvent);
        }
    }

    /// <summary>
    /// An event used to trigger vignette shakes
    /// </summary>
    public struct MMWhiteBalanceShakeEvent_HDRP
    {
        public delegate void Delegate(AnimationCurve temperature, float duration, float remapTemperatureMin, float remapTemperatureMax,
            AnimationCurve tint, float remapTintMin, float remapTintMax, bool relativeValues = false,
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

        static public void Trigger(AnimationCurve temperature, float duration, float remapTemperatureMin, float remapTemperatureMax,
            AnimationCurve tint, float remapTintMin, float remapTintMax, bool relativeValues = false,
            float attenuation = 1.0f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
        {
            OnEvent?.Invoke(temperature, duration, remapTemperatureMin, remapTemperatureMax,
                tint, remapTintMin, remapTintMax, relativeValues,
                attenuation, channel, resetShakerValuesAfterShake, resetTargetValuesAfterShake, forwardDirection, timescaleMode, stop);
        }
    }
}
