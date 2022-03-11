using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using System;

namespace MoreMountains.Feedbacks
{
    public struct MMFlashEvent
    {
        public delegate void Delegate(Color flashColor, float duration, float alpha, int flashID, int channel, TimescaleModes timescaleMode, bool stop = false);
        static private event Delegate OnEvent;

        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        static public void Trigger(Color flashColor, float duration, float alpha, int flashID, int channel, TimescaleModes timescaleMode, bool stop = false)
        {
            OnEvent?.Invoke(flashColor, duration, alpha, flashID, channel, timescaleMode, stop);
        }
    }

    [Serializable]
    public class MMFlashDebugSettings
    {
        /// the channel to broadcast that flash event on
        public int Channel = 0;
        /// the color of the flash
        public Color FlashColor = Color.white;
        /// the flash duration (in seconds)
        public float FlashDuration = 0.2f;
        /// the alpha of the flash
        public float FlashAlpha = 1f;
        /// the ID of the flash (usually 0). You can specify on each MMFlash object an ID, allowing you to have different flash images in one scene and call them separately (one for damage, one for health pickups, etc)
        public int FlashID = 0;
    }
    
	[RequireComponent(typeof(Image))]
    [RequireComponent(typeof(CanvasGroup))]
    [AddComponentMenu("More Mountains/Feedbacks/Shakers/Various/MMFlash")]
    /// <summary>
    /// Add this class to an image and it'll flash when getting a MMFlashEvent
    /// </summary>
    public class MMFlash : MonoBehaviour
    {
        [Header("Flash")]
        /// the channel to receive flash events on
        [Tooltip("the channel to receive flash events on")]
        public int Channel = 0;
        /// the ID of this MMFlash object. When triggering a MMFlashEvent you can specify an ID, and only MMFlash objects with this ID will answer the call and flash, allowing you to have more than one flash object in a scene
        [Tooltip("the ID of this MMFlash object. When triggering a MMFlashEvent you can specify an ID, and only MMFlash objects with this ID will answer the call and flash, allowing you to have more than one flash object in a scene")]
        public int FlashID = 0;
        
        [Header("Debug")]
        /// the set of test settings to use when pressing the DebugTest button
        [Tooltip("the set of test settings to use when pressing the DebugTest button")]
        public MMFlashDebugSettings DebugSettings;
        /// a test button that calls the DebugTest method
        [Tooltip("a test button that calls the DebugTest method")]
        [MMFInspectorButton("DebugTest")]
        public bool DebugTestButton;

        public virtual float GetTime() { return (_timescaleMode == TimescaleModes.Scaled) ? Time.time : Time.unscaledTime; }
        public virtual float GetDeltaTime() { return (_timescaleMode == TimescaleModes.Scaled) ? Time.deltaTime : Time.unscaledDeltaTime; }

        protected Image _image;
        protected CanvasGroup _canvasGroup;
		protected bool _flashing = false;
        protected float _targetAlpha;
        protected Color _initialColor;
        protected float _delta;
        protected float _flashStartedTimestamp;
        protected int _direction = 1;
        protected float _duration;
        protected TimescaleModes _timescaleMode;

		/// <summary>
		/// On start we grab our image component
		/// </summary>
		protected virtual void Start()
		{
			_image = GetComponent<Image>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _initialColor = _image.color;
        }

		/// <summary>
		/// On update we flash our image if needed
		/// </summary>
		protected virtual void Update()
		{
			if (_flashing)
			{
				_image.enabled = true;

                if (GetTime() - _flashStartedTimestamp > _duration / 2f)
                {
                    _direction = -1;
                }

                if (_direction == 1)
                {
                    _delta += GetDeltaTime() / (_duration / 2f);
                }
                else
                {
                    _delta -= GetDeltaTime() / (_duration / 2f);
                }
                
                if (GetTime() - _flashStartedTimestamp > _duration)
                {
                    _flashing = false;
                }

                _canvasGroup.alpha = Mathf.Lerp(0f, _targetAlpha, _delta);
            }
			else
			{
				_image.enabled = false;
			}
		}

        public virtual void DebugTest()
        {
            MMFlashEvent.Trigger(DebugSettings.FlashColor, DebugSettings.FlashDuration, DebugSettings.FlashAlpha, DebugSettings.FlashID, DebugSettings.Channel, TimescaleModes.Unscaled);
        }

		/// <summary>
		/// When getting a flash event, we turn our image on
		/// </summary>
		public virtual void OnMMFlashEvent(Color flashColor, float duration, float alpha, int flashID, int channel, TimescaleModes timescaleMode, bool stop = false)
        {
            if (flashID != FlashID) 
            {
                return;
            }
            
            if (stop)
            {
	            _flashing = false;
	            return;
            }

            if ((channel != Channel) && (channel != -1) && (Channel != -1))
            {
                return;
            }

            if (!_flashing)
            {
                _flashing = true;
                _direction = 1;
                _canvasGroup.alpha = 0;
                _targetAlpha = alpha;
                _delta = 0f;
                _image.color = flashColor;
                _duration = duration;
                _timescaleMode = timescaleMode;
                _flashStartedTimestamp = GetTime();
            }
        } 

		/// <summary>
		/// On enable we start listening for events
		/// </summary>
		protected virtual void OnEnable()
		{
            MMFlashEvent.Register(OnMMFlashEvent);
		}

		/// <summary>
		/// On disable we stop listening for events
		/// </summary>
		protected virtual void OnDisable()
		{
            MMFlashEvent.Unregister(OnMMFlashEvent);
        }		
	}
}