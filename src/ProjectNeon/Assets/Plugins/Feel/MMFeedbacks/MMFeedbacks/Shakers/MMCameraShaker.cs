using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using System;

namespace MoreMountains.Feedbacks
{
	[Serializable]
	/// <summary>
	/// Camera shake properties
	/// </summary>
	public struct MMCameraShakeProperties
	{
		public float Duration;
		public float Amplitude;
		public float Frequency;
        public float AmplitudeX;
        public float AmplitudeY;
        public float AmplitudeZ;

        public MMCameraShakeProperties(float duration, float amplitude, float frequency, float amplitudeX = 0f, float amplitudeY = 0f, float amplitudeZ = 0f)
        {
            Duration = duration;
            Amplitude = amplitude;
            Frequency = frequency;
            AmplitudeX = amplitudeX;
            AmplitudeY = amplitudeY;
            AmplitudeZ = amplitudeZ;
        }
    }

    public enum MMCameraZoomModes { For, Set, Reset }

    public struct MMCameraZoomEvent
    {
        public delegate void Delegate(MMCameraZoomModes mode, float newFieldOfView, float transitionDuration, float duration, int channel, bool useUnscaledTime = false, bool stop = false, bool relative = false);
        
        static private event Delegate OnEvent;

        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        static public void Trigger(MMCameraZoomModes mode, float newFieldOfView, float transitionDuration, float duration, int channel, bool useUnscaledTime = false, bool stop = false, bool relative = false)
        {
            OnEvent?.Invoke(mode, newFieldOfView, transitionDuration, duration, channel, useUnscaledTime, stop, relative);
        }
    }

    public struct MMCameraShakeEvent
    {
        public delegate void Delegate(float duration, float amplitude, float frequency, float amplitudeX, float amplitudeY, float amplitudeZ, bool infinite = false, int channel = 0, bool useUnscaledTime = false);
        static private event Delegate OnEvent;

        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        static public void Trigger(float duration, float amplitude, float frequency, float amplitudeX, float amplitudeY, float amplitudeZ, bool infinite = false, int channel = 0, bool useUnscaledTime = false)
        {
            OnEvent?.Invoke(duration, amplitude, frequency, amplitudeX, amplitudeY, amplitudeZ, infinite, channel, useUnscaledTime);
        }
    }

    public struct MMCameraShakeStopEvent
    {
        public delegate void Delegate(int channel);
        static private event Delegate OnEvent;

        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        static public void Trigger(int channel)
        {
            OnEvent?.Invoke(channel);
        }
    }

    [RequireComponent(typeof(MMWiggle))]
    [AddComponentMenu("More Mountains/Feedbacks/Shakers/Camera/MMCameraShaker")]
    /// <summary>
    /// A class to add to your camera. It'll listen to MMCameraShakeEvents and will shake your camera accordingly
    /// </summary>
    public class MMCameraShaker : MonoBehaviour
    {
	    /// the channel to broadcast this shake on
	    [Tooltip("the channel to broadcast this shake on")]
        public int Channel = 0;
	    
        protected MMWiggle _wiggle;

		/// <summary>
		/// On Awake, grabs the MMShaker component
		/// </summary>
		protected virtual void Awake()
		{
			_wiggle = GetComponent<MMWiggle>();
		}

		/// <summary>
		/// Shakes the camera for Duration seconds, by the desired amplitude and frequency
		/// </summary>
		/// <param name="duration">Duration.</param>
		/// <param name="amplitude">Amplitude.</param>
		/// <param name="frequency">Frequency.</param>
		public virtual void ShakeCamera(float duration, float amplitude, float frequency, float amplitudeX, float amplitudeY, float amplitudeZ, bool useUnscaledTime)
		{
            if ((amplitudeX != 0f) || (amplitudeY != 0f) || (amplitudeZ != 0f))
            {
                _wiggle.PositionWiggleProperties.AmplitudeMin.x = -amplitudeX;
                _wiggle.PositionWiggleProperties.AmplitudeMin.y = -amplitudeY;
                _wiggle.PositionWiggleProperties.AmplitudeMin.z = -amplitudeZ;
                
                _wiggle.PositionWiggleProperties.AmplitudeMax.x = amplitudeX;
                _wiggle.PositionWiggleProperties.AmplitudeMax.y = amplitudeY;
                _wiggle.PositionWiggleProperties.AmplitudeMax.z = amplitudeZ;
            }
            else
            {
                _wiggle.PositionWiggleProperties.AmplitudeMin = Vector3.one * -amplitude;
                _wiggle.PositionWiggleProperties.AmplitudeMax = Vector3.one * amplitude;
            }
            
            _wiggle.PositionWiggleProperties.UseUnscaledTime = useUnscaledTime;
            _wiggle.PositionWiggleProperties.FrequencyMin = frequency;
            _wiggle.PositionWiggleProperties.FrequencyMax = frequency;
            _wiggle.PositionWiggleProperties.NoiseFrequencyMin = frequency * Vector3.one;
            _wiggle.PositionWiggleProperties.NoiseFrequencyMax = frequency * Vector3.one;
            _wiggle.WigglePosition(duration);
		}

		/// <summary>
		/// When a MMCameraShakeEvent is caught, shakes the camera
		/// </summary>
		/// <param name="shakeEvent">Shake event.</param>
		public virtual void OnCameraShakeEvent(float duration, float amplitude, float frequency, float amplitudeX, float amplitudeY, float amplitudeZ, bool infinite, int channel, bool useUnscaledTime)
		{
            if (channel != Channel)
            {
                return;
            }
			this.ShakeCamera (duration, amplitude, frequency, amplitudeX, amplitudeY, amplitudeZ, useUnscaledTime);
		}

		/// <summary>
		/// On enable, starts listening for events
		/// </summary>
		protected virtual void OnEnable()
		{
            MMCameraShakeEvent.Register(OnCameraShakeEvent);
        }

		/// <summary>
		/// On disable, stops listening to events
		/// </summary>
		protected virtual void OnDisable()
		{
            MMCameraShakeEvent.Unregister(OnCameraShakeEvent);
        }

	}
}