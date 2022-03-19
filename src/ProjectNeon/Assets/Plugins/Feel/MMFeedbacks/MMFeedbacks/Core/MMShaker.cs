using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace MoreMountains.Feedbacks
{
    public class MMShaker : MonoBehaviour
    {
        [Header("Shake Settings")]
        /// the channel to listen to - has to match the one on the feedback
        [Tooltip("the channel to listen to - has to match the one on the feedback")]
        public int Channel = 0;
        /// the duration of the shake, in seconds
        [Tooltip("the duration of the shake, in seconds")]
        public float ShakeDuration = 0.2f;
        /// if this is true this shaker will play on awake
        [Tooltip("if this is true this shaker will play on awake")]
        public bool PlayOnAwake = false;
        /// if this is true, a new shake can happen while shaking
        [Tooltip("if this is true, a new shake can happen while shaking")]
        public bool Interruptible = true;
        /// if this is true, this shaker will always reset target values, regardless of how it was called
        [Tooltip("if this is true, this shaker will always reset target values, regardless of how it was called")]
        public bool AlwaysResetTargetValuesAfterShake = false;
        /// whether or not this shaker is shaking right now
        [Tooltip("whether or not this shaker is shaking right now")]
        [MMFReadOnly]
        public bool Shaking = false;
        
        [HideInInspector] 
        public bool ForwardDirection = true;

        [HideInInspector] 
        public TimescaleModes TimescaleMode = TimescaleModes.Scaled;
        
        
        public virtual float GetTime() { return (TimescaleMode == TimescaleModes.Scaled) ? Time.time : Time.unscaledTime; }
        public virtual float GetDeltaTime() { return (TimescaleMode == TimescaleModes.Scaled) ? Time.deltaTime : Time.unscaledDeltaTime; }
        
        public bool ListeningToEvents => _listeningToEvents;

        [HideInInspector]
        internal bool _listeningToEvents = false;
        protected float _shakeStartedTimestamp;
        protected float _remappedTimeSinceStart;
        protected bool _resetShakerValuesAfterShake;
        protected bool _resetTargetValuesAfterShake;
        protected float _journey;
        
        /// <summary>
        /// On Awake we grab our volume and profile
        /// </summary>
        protected virtual void Awake()
        {
            Shaking = false;
            Initialization();
            // in case someone else trigger StartListening before Awake
            if (!_listeningToEvents)
            {
                StartListening();
            }
            this.enabled = PlayOnAwake;
        }

        /// <summary>
        /// Override this method to initialize your shaker
        /// </summary>
        protected virtual void Initialization()
        {
        }

        /// <summary>
        /// Starts shaking the values
        /// </summary>
        public virtual void StartShaking()
        {
            _journey = ForwardDirection ? 0f : ShakeDuration;
            
            if (Shaking)
            {
                return;
            }
            else
            {
                this.enabled = true;
                _shakeStartedTimestamp = GetTime();
                Shaking = true;
                GrabInitialValues();
                ShakeStarts();
            }
        }

        /// <summary>
        /// Describes what happens when a shake starts
        /// </summary>
        protected virtual void ShakeStarts()
        {

        }

        /// <summary>
        /// A method designed to collect initial values
        /// </summary>
        protected virtual void GrabInitialValues()
        {

        }

        /// <summary>
        /// On Update, we shake our values if needed, or reset if our shake has ended
        /// </summary>
        protected virtual void Update()
        {
            if (Shaking)
            {
                Shake();
                _journey += ForwardDirection ? GetDeltaTime() : -GetDeltaTime();
            }

            if (Shaking && ((_journey < 0) || (_journey > ShakeDuration)))
            {
                Shaking = false;
                ShakeComplete();
            }
        }

        /// <summary>
        /// Override this method to implement shake over time
        /// </summary>
        protected virtual void Shake()
        {

        }

        /// <summary>
        /// A method used to "shake" a flot over time along a curve
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="remapMin"></param>
        /// <param name="remapMax"></param>
        /// <param name="relativeIntensity"></param>
        /// <param name="initialValue"></param>
        /// <returns></returns>
        protected virtual float ShakeFloat(AnimationCurve curve, float remapMin, float remapMax, bool relativeIntensity, float initialValue)
        {
            float newValue = 0f;
            
            float remappedTime = MMFeedbacksHelpers.Remap(_journey, 0f, ShakeDuration, 0f, 1f);
            
            float curveValue = curve.Evaluate(remappedTime);
            newValue = MMFeedbacksHelpers.Remap(curveValue, 0f, 1f, remapMin, remapMax);
            if (relativeIntensity)
            {
                newValue += initialValue;
            }
            return newValue;
        }

        /// <summary>
        /// Resets the values on the target
        /// </summary>
        protected virtual void ResetTargetValues()
        {

        }

        /// <summary>
        /// Resets the values on the shaker
        /// </summary>
        protected virtual void ResetShakerValues()
        {

        }

        /// <summary>
        /// Describes what happens when the shake is complete
        /// </summary>
        protected virtual void ShakeComplete()
        {
            if (_resetTargetValuesAfterShake || AlwaysResetTargetValuesAfterShake)
            {
                ResetTargetValues();
            }   
            if (_resetShakerValuesAfterShake)
            {
                ResetShakerValues();
            }            
            this.enabled = false;
        }

        /// <summary>
        /// On enable we start shaking if needed
        /// </summary>
        protected virtual void OnEnable()
        {
            StartShaking();
        }
             
        /// <summary>
        /// On destroy we stop listening for events
        /// </summary>
        protected virtual void OnDestroy()
        {
            StopListening();
        }

        /// <summary>
        /// On disable we complete our shake if it was in progress
        /// </summary>
        protected virtual void OnDisable()
        {
            if (Shaking)
            {
                ShakeComplete();
            }
        }

        /// <summary>
        /// Starts this shaker
        /// </summary>
        public virtual void Play()
        {
            this.enabled = true;
        }

        /// <summary>
        /// Stops this shaker
        /// </summary>
        public virtual void Stop()
        {
            Shaking = false;
            ShakeComplete();
        }
        
        /// <summary>
        /// Starts listening for events
        /// </summary>
        public virtual void StartListening()
        {
            _listeningToEvents = true;
        }

        /// <summary>
        /// Stops listening for events
        /// </summary>
        public virtual void StopListening()
        {
            _listeningToEvents = false;
        }

        /// <summary>
        /// Returns true if this shaker should listen to events, false otherwise
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        protected virtual bool CheckEventAllowed(int channel, bool useRange = false, float range = 0f, Vector3 eventOriginPosition = default(Vector3))
        {
            if ((channel != Channel) && (channel != -1) && (Channel != -1))
            {
                return false;
            }
            if (!this.gameObject.activeInHierarchy)
            {
                return false;
            }
            else
            {
                if (useRange)
                {
                    if (Vector3.Distance(this.transform.position, eventOriginPosition) > range)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
