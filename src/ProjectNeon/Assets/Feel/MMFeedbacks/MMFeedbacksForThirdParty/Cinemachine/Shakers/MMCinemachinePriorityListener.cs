using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// Add this to a Cinemachine virtual camera and it'll be able to listen to MMCinemachinePriorityEvent, usually triggered by a MMFeedbackCinemachineTransition
    /// </summary>
    [AddComponentMenu("More Mountains/Feedbacks/Shakers/Cinemachine/MMCinemachinePriorityListener")]
    [RequireComponent(typeof(CinemachineVirtualCameraBase))]
    public class MMCinemachinePriorityListener : MonoBehaviour
    {
        
        [HideInInspector] 
        public TimescaleModes TimescaleMode = TimescaleModes.Scaled;
        
        
        public virtual float GetTime() { return (TimescaleMode == TimescaleModes.Scaled) ? Time.time : Time.unscaledTime; }
        public virtual float GetDeltaTime() { return (TimescaleMode == TimescaleModes.Scaled) ? Time.deltaTime : Time.unscaledDeltaTime; }
        
        [Header("Priority Listener")]
        /// the channel to listen to
        [Tooltip("the channel to listen to")]
        public int Channel = 0;

        protected CinemachineVirtualCameraBase _camera;
        
        /// <summary>
        /// On Awake we store our virtual camera
        /// </summary>
        protected virtual void Awake()
        {
            _camera = this.gameObject.GetComponent<CinemachineVirtualCameraBase>();
        }

        /// <summary>
        /// When we get an event we change our priorities if needed
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="forceMaxPriority"></param>
        /// <param name="newPriority"></param>
        /// <param name="forceTransition"></param>
        /// <param name="blendDefinition"></param>
        /// <param name="resetValuesAfterTransition"></param>
        public virtual void OnMMCinemachinePriorityEvent(int channel, bool forceMaxPriority, int newPriority, bool forceTransition, CinemachineBlendDefinition blendDefinition, bool resetValuesAfterTransition, TimescaleModes timescaleMode)
        {
            TimescaleMode = timescaleMode;
            if (channel == Channel)
            {
                _camera.Priority = newPriority;
            }
            else
            {
                if (forceMaxPriority)
                {
                    _camera.Priority = 0;
                }
            }
        }

        /// <summary>
        /// On enable we start listening for events
        /// </summary>
        protected virtual void OnEnable()
        {
            MMCinemachinePriorityEvent.Register(OnMMCinemachinePriorityEvent);
        }

        /// <summary>
        /// Stops listening for events
        /// </summary>
        protected virtual void OnDisable()
        {
            MMCinemachinePriorityEvent.Unregister(OnMMCinemachinePriorityEvent);
        }
    }

    /// <summary>
    /// An event used to pilot priorities on cinemachine virtual cameras and brain transitions
    /// </summary>
    public struct MMCinemachinePriorityEvent
    {
        public delegate void Delegate(int channel, bool forceMaxPriority, int newPriority, bool forceTransition, CinemachineBlendDefinition blendDefinition, bool resetValuesAfterTransition, TimescaleModes timescaleMode);
        static private event Delegate OnEvent;

        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }

        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }

        static public void Trigger(int channel, bool forceMaxPriority, int newPriority, bool forceTransition, CinemachineBlendDefinition blendDefinition, bool resetValuesAfterTransition, TimescaleModes timescaleMode)
        {
            OnEvent?.Invoke(channel, forceMaxPriority, newPriority, forceTransition, blendDefinition, resetValuesAfterTransition, timescaleMode);
        }
    }
}
