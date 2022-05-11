using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// Turns an object active or inactive at the various stages of the feedback
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback allows you to change the state of the target gameobject from active to inactive (or the opposite), on init, play, stop or reset. For each of these you can specify if you want to force a state (active or inactive), or toggle it (active becomes inactive, inactive becomes active).")]
    [FeedbackPath("GameObject/Set Active")]
    public class MMF_SetActive : MMF_Feedback
    {
        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;
        /// sets the inspector color for this feedback
        #if UNITY_EDITOR
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.GameObjectColor; } }
        public override bool EvaluateRequiresSetup() { return (TargetGameObject == null); }
        public override string RequiredTargetText { get { return TargetGameObject != null ? TargetGameObject.name : "";  } }
        public override string RequiresSetupText { get { return "This feedback requires that a TargetGameObject be set to be able to work properly. You can set one below."; } }
        #endif

        /// the possible effects the feedback can have on the target object's status 
        public enum PossibleStates { Active, Inactive, Toggle }
        
        [MMFInspectorGroup("Set Active Target", true, 12, true)]
        /// the gameobject we want to change the active state of
        [Tooltip("the gameobject we want to change the active state of")]
        public GameObject TargetGameObject;
        
        [MMFInspectorGroup("States", true, 14)]
        /// whether or not we should alter the state of the target object on init
        [Tooltip("whether or not we should alter the state of the target object on init")]
        public bool SetStateOnInit = false;
        [MMFCondition("SetStateOnInit", true)]
        /// how to change the state on init
        [Tooltip("how to change the state on init")]
        public PossibleStates StateOnInit = PossibleStates.Inactive;
        /// whether or not we should alter the state of the target object on play
        [Tooltip("whether or not we should alter the state of the target object on play")]
        public bool SetStateOnPlay = false;
        /// how to change the state on play
        [Tooltip("how to change the state on play")]
        [MMFCondition("SetStateOnPlay", true)]
        public PossibleStates StateOnPlay = PossibleStates.Inactive;
        /// whether or not we should alter the state of the target object on stop
        [Tooltip("whether or not we should alter the state of the target object on stop")]
        public bool SetStateOnStop = false;
        /// how to change the state on stop
        [Tooltip("how to change the state on stop")]
        [MMFCondition("SetStateOnStop", true)]
        public PossibleStates StateOnStop = PossibleStates.Inactive;
        /// whether or not we should alter the state of the target object on reset
        [Tooltip("whether or not we should alter the state of the target object on reset")]
        public bool SetStateOnReset = false;
        /// how to change the state on reset
        [Tooltip("how to change the state on reset")]
        [MMFCondition("SetStateOnReset", true)]
        public PossibleStates StateOnReset = PossibleStates.Inactive;
        
        
        /// <summary>
        /// On init we change the state of our object if needed
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(MMF_Player owner)
        {
            base.CustomInitialization(owner);
            if (Active && (TargetGameObject != null))
            {
                if (SetStateOnInit)
                {
                    SetStatus(StateOnInit);
                }
            }
        }

        /// <summary>
        /// On Play we change the state of our object if needed
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized || (TargetGameObject == null))
            {
                return;
            }
            
            if (SetStateOnPlay)
            {
                SetStatus(StateOnPlay);
            }
        }

        /// <summary>
        /// On Stop we change the state of our object if needed
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            base.CustomStopFeedback(position, feedbacksIntensity);

            if (Active && FeedbackTypeAuthorized && (TargetGameObject != null))
            {
                if (SetStateOnStop)
                {
                    SetStatus(StateOnStop);
                }
            }
        }

        /// <summary>
        /// On Reset we change the state of our object if needed
        /// </summary>
        protected override void CustomReset()
        {
            base.CustomReset();

            if (InCooldown)
            {
                return;
            }

            if (Active && FeedbackTypeAuthorized && (TargetGameObject != null))
            {
                if (SetStateOnReset)
                {
                    SetStatus(StateOnReset);
                }
            }
        }

        /// <summary>
        /// Changes the status of the object
        /// </summary>
        /// <param name="state"></param>
        protected virtual void SetStatus(PossibleStates state)
        {
            bool newState = false;
            switch (state)
            {
                case PossibleStates.Active:
                    newState = NormalPlayDirection ? true : false;
                    break;
                case PossibleStates.Inactive:
                    newState = NormalPlayDirection ? false : true;
                    break;
                case PossibleStates.Toggle:
                    newState = !TargetGameObject.activeInHierarchy;
                    break;
            }
            TargetGameObject.SetActive(newState);
        }
    }
}
