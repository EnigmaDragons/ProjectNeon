using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MoreMountains.Feedbacks
{
    [Serializable]
    public abstract class MMF_Feedback
    {
        #region Properties

        [MMFInspectorGroup("Feedback Settings", true, 0, false, true)]
        /// whether or not this feedback is active
        [Tooltip("whether or not this feedback is active")]
        public bool Active = true;
        [HideInInspector]
        public int UniqueID;
        /// the name of this feedback to display in the inspector
        [Tooltip("the name of this feedback to display in the inspector")]
        public string Label = "MMFeedback";
        /// the ID of the channel on which this feedback will communicate 
        [Tooltip("the ID of the channel on which this feedback will communicate")]
        public int Channel = 0;
        /// the chance of this feedback happening (in percent : 100 : happens all the time, 0 : never happens, 50 : happens once every two calls, etc)
        [Tooltip("the chance of this feedback happening (in percent : 100 : happens all the time, 0 : never happens, 50 : happens once every two calls, etc)")]
        [Range(0,100)]
        public float Chance = 100f;
        /// use this color to customize the background color of the feedback in the MMF_Player's list
        [Tooltip("use this color to customize the background color of the feedback in the MMF_Player's list")]
        public Color DisplayColor = Color.black;
        /// a number of timing-related values (delay, repeat, etc)
        [Tooltip("a number of timing-related values (delay, repeat, etc)")]
        public MMFeedbackTiming Timing;
        /// the Owner of the feedback, as defined when calling the Initialization method
        [HideInInspector]
        public MMF_Player Owner;
        [HideInInspector]
        /// whether or not this feedback is in debug mode
        public bool DebugActive = false;
        /// set this to true if your feedback should pause the execution of the feedback sequence
        public virtual IEnumerator Pause => null;
        /// if this is true, this feedback will wait until all previous feedbacks have run
        public virtual bool HoldingPause => false;
        /// if this is true, this feedback will wait until all previous feedbacks have run, then run all previous feedbacks again
        public virtual bool LooperPause => false;
        /// if this is true, this feedback will pause and wait until Resume() is called on its parent MMFeedbacks to resume execution
        public virtual bool ScriptDrivenPause { get; set; }
        /// if this is a positive value, the feedback will auto resume after that duration if it hasn't been resumed via script already
        public virtual float ScriptDrivenPauseAutoResume { get; set; }
        /// if this is true, this feedback will wait until all previous feedbacks have run, then run all previous feedbacks again
        public virtual bool LooperStart => false;
        /// if this is true, the Channel property will be displayed, otherwise it'll be hidden        
        public virtual bool HasChannel => false;
        public virtual bool HasCustomInspectors => false;
        /// an overridable color for your feedback, that can be redefined per feedback. White is the only reserved color, and the feedback will revert to 
        /// normal (light or dark skin) when left to White
        #if UNITY_EDITOR
            public virtual Color FeedbackColor { get => Color.white; }
        #endif
        /// returns true if this feedback is in cooldown at this time (and thus can't play), false otherwise
        public virtual bool InCooldown { get { return (Timing.CooldownDuration > 0f) && (FeedbackTime - _lastPlayTimestamp < Timing.CooldownDuration); } }
        /// if this is true, this feedback is currently playing
        public virtual bool IsPlaying { get; set; }
        
        /// the time (or unscaled time) based on the selected Timing settings
        public virtual float FeedbackTime 
        { 
            get 
            {
                if (Timing.TimescaleMode == TimescaleModes.Scaled)
                {
                    return Time.time;
                }
                else
                {
                    return Time.unscaledTime;
                }
            } 
        }
        
        /// the delta time (or unscaled delta time) based on the selected Timing settings
        public virtual float FeedbackDeltaTime
        {
            get
            {
                if (Owner.SkippingToTheEnd)
                {
                    return float.MaxValue;
                }
                if (Timing.TimescaleMode == TimescaleModes.Scaled)
                {
                    return Time.deltaTime;
                }
                else
                {
                    return Time.unscaledDeltaTime;
                }
            }
        }

        
        /// <summary>
        /// The total duration of this feedback :
        /// total = initial delay + duration * (number of repeats + delay between repeats)  
        /// </summary>
        public virtual float TotalDuration
        {
            get
            {
                if ((Timing != null) && (!Timing.ContributeToTotalDuration))
                {
                    return 0f;
                }
                float totalTime = 0f;

                if (Timing == null)
                {
                    return 0f;
                }
                
                if (Timing.InitialDelay != 0)
                {
                    totalTime += ApplyTimeMultiplier(Timing.InitialDelay);
                }
            
                totalTime += FeedbackDuration;

                if (Timing.NumberOfRepeats != 0)
                {
                    float delayBetweenRepeats = ApplyTimeMultiplier(Timing.DelayBetweenRepeats); 
                    
                    totalTime += (Timing.NumberOfRepeats * FeedbackDuration) + (Timing.NumberOfRepeats  * delayBetweenRepeats);
                }

                return totalTime;
            }
        }
        
        /// <summary>
        /// A flag used to determine if a feedback has all it needs, or if it requires some extra setup.
        /// This flag will be used to display a warning icon in the inspector if the feedback is not ready to be played.
        /// </summary>
        public bool RequiresSetup { get { return _requiresSetup;  } }
        public string RequiredTarget { get { return _requiredTarget;  } }
        public virtual void CacheRequiresSetup() { _requiresSetup = EvaluateRequiresSetup(); _requiredTarget = RequiredTargetText == "" ? "" : "["+RequiredTargetText+"]"; }
        public virtual bool DrawGroupInspectors { get { return true;  } }
        
        public virtual string RequiresSetupText { get { return "This feedback requires some additional setup."; } }
        public virtual string RequiredTargetText { get { return ""; } }
        
        /// <summary>
        /// Override this method to determine if a feedback requires setup 
        /// </summary>
        /// <returns></returns>
        public virtual bool EvaluateRequiresSetup() { return false;  }

        // the timestamp at which this feedback was last played
        public virtual float FeedbackStartedAt { get { return Application.isPlaying ? _lastPlayTimestamp : -1f; } }
        // the perceived duration of the feedback, to be used to display its progress bar, meant to be overridden with meaningful data by each feedback
        public virtual float FeedbackDuration { get { return 0f; } set { } }
        /// whether or not this feedback is playing right now
        public virtual bool FeedbackPlaying { get { return ((FeedbackStartedAt > 0f) && (Time.time - FeedbackStartedAt < FeedbackDuration)); } }

        protected float _lastPlayTimestamp = -1f;
        protected int _playsLeft;
        protected bool _initialized = false;
        protected Coroutine _playCoroutine;
        protected Coroutine _infinitePlayCoroutine;
        protected Coroutine _sequenceCoroutine;
        protected Coroutine _repeatedPlayCoroutine;
        protected bool _requiresSetup = false;
        protected string _requiredTarget = "";
        
        protected int _sequenceTrackID = 0;
        protected float _beatInterval;
        protected bool BeatThisFrame = false;
        protected int LastBeatIndex = 0;
        protected int CurrentSequenceIndex = 0;
        protected float LastBeatTimestamp = 0f;

        #endregion Properties
        
        #region Initialization

        /// <summary>
        /// Initializes the feedback and its timing related variables
        /// </summary>
        /// <param name="owner"></param>
        public virtual void Initialization(MMF_Player owner)
        {
            _lastPlayTimestamp = -1f;
            _initialized = true;
            Owner = owner;
            _playsLeft = Timing.NumberOfRepeats + 1;
            
            SetInitialDelay(Timing.InitialDelay);
            SetDelayBetweenRepeats(Timing.DelayBetweenRepeats);
            SetSequence(Timing.Sequence);

            CustomInitialization(owner);            
        }

        #endregion Initialization
        
        #region Play
        
        /// <summary>
        /// Plays the feedback
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        public virtual void Play(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active)
            {
                return;
            }

            if (!_initialized)
            {
                Debug.LogWarning("The " + this + " feedback is being played without having been initialized. Call Initialization() first.");
            }
            
            // we check the cooldown
            if (InCooldown)
            {
                return;
            }

            if (Timing.InitialDelay > 0f) 
            {
                _playCoroutine = Owner.StartCoroutine(PlayCoroutine(position, feedbacksIntensity));
            }
            else
            {
                _lastPlayTimestamp = FeedbackTime;
                RegularPlay(position, feedbacksIntensity);
            }  
        }
        
        /// <summary>
        /// An internal coroutine delaying the initial play of the feedback
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        /// <returns></returns>
        protected virtual IEnumerator PlayCoroutine(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (Timing.TimescaleMode == TimescaleModes.Scaled)
            {
                yield return MMFeedbacksCoroutine.WaitFor(Timing.InitialDelay);
            }
            else
            {
                yield return MMFeedbacksCoroutine.WaitForUnscaled(Timing.InitialDelay);
            }
            _lastPlayTimestamp = FeedbackTime;
            RegularPlay(position, feedbacksIntensity);
        }

        /// <summary>
        /// Triggers delaying coroutines if needed
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected virtual void RegularPlay(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (Chance == 0f)
            {
                return;
            }
            if (Chance != 100f)
            {
                // determine the odds
                float random = Random.Range(0f, 100f);
                if (random > Chance)
                {
                    return;
                }
            }

            if (Timing.UseIntensityInterval)
            {
                if ((feedbacksIntensity < Timing.IntensityIntervalMin) || (feedbacksIntensity >= Timing.IntensityIntervalMax))
                {
                    return;
                }
            }

            if (Timing.RepeatForever)
            {
                _infinitePlayCoroutine = Owner.StartCoroutine(InfinitePlay(position, feedbacksIntensity));
                return;
            }
            if (Timing.NumberOfRepeats > 0)
            {
                _repeatedPlayCoroutine = Owner.StartCoroutine(RepeatedPlay(position, feedbacksIntensity));
                return;
            }            
            if (Timing.Sequence == null)
            {
                CustomPlayFeedback(position, feedbacksIntensity);
            }
            else
            {
                _sequenceCoroutine = Owner.StartCoroutine(SequenceCoroutine(position, feedbacksIntensity));
            }
            
        }

        /// <summary>
        /// Internal coroutine used for repeated play without end
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        /// <returns></returns>
        protected virtual IEnumerator InfinitePlay(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            while (true)
            {
                _lastPlayTimestamp = FeedbackTime;
                if (Timing.Sequence == null)
                {
                    CustomPlayFeedback(position, feedbacksIntensity);
                    if (Timing.TimescaleMode == TimescaleModes.Scaled)
                    {
                        yield return MMFeedbacksCoroutine.WaitFor(Timing.DelayBetweenRepeats);
                    }
                    else
                    {
                        yield return MMFeedbacksCoroutine.WaitForUnscaled(Timing.DelayBetweenRepeats);
                    }
                }
                else
                {
                    _sequenceCoroutine = Owner.StartCoroutine(SequenceCoroutine(position, feedbacksIntensity));

                    float delay = ApplyTimeMultiplier(Timing.DelayBetweenRepeats) + Timing.Sequence.Length;
                    if (Timing.TimescaleMode == TimescaleModes.Scaled)
                    {
                        yield return MMFeedbacksCoroutine.WaitFor(delay);
                    }
                    else
                    {
                        yield return MMFeedbacksCoroutine.WaitForUnscaled(delay);
                    }
                }
            }
        }

        /// <summary>
        /// Internal coroutine used for repeated play
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        /// <returns></returns>
        protected virtual IEnumerator RepeatedPlay(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            while (_playsLeft > 0)
            {
                _lastPlayTimestamp = FeedbackTime;
                _playsLeft--;
                if (Timing.Sequence == null)
                {
                    CustomPlayFeedback(position, feedbacksIntensity);
                    
                    if (Timing.TimescaleMode == TimescaleModes.Scaled)
                    {
                        yield return MMFeedbacksCoroutine.WaitFor(Timing.DelayBetweenRepeats);
                    }
                    else
                    {
                        yield return MMFeedbacksCoroutine.WaitForUnscaled(Timing.DelayBetweenRepeats);
                    }
                }
                else
                {
                    _sequenceCoroutine = Owner.StartCoroutine(SequenceCoroutine(position, feedbacksIntensity));
                    
                    float delay = ApplyTimeMultiplier(Timing.DelayBetweenRepeats) + Timing.Sequence.Length;
                    if (Timing.TimescaleMode == TimescaleModes.Scaled)
                    {
                        yield return MMFeedbacksCoroutine.WaitFor(delay);
                    }
                    else
                    {
                        yield return MMFeedbacksCoroutine.WaitForUnscaled(delay);
                    }
                }
            }
            _playsLeft = Timing.NumberOfRepeats + 1;
        }

        #endregion Play

        #region Sequence

        /// <summary>
        /// A coroutine used to play this feedback on a sequence
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        /// <returns></returns>
        protected virtual IEnumerator SequenceCoroutine(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            yield return null;
            float timeStartedAt = FeedbackTime;
            float lastFrame = FeedbackTime;

            BeatThisFrame = false;
            LastBeatIndex = 0;
            CurrentSequenceIndex = 0;
            LastBeatTimestamp = 0f;

            if (Timing.Quantized)
            {
                while (CurrentSequenceIndex < Timing.Sequence.QuantizedSequence[0].Line.Count)
                {
                    _beatInterval = 60f / Timing.TargetBPM;

                    if ((FeedbackTime - LastBeatTimestamp >= _beatInterval) || (LastBeatTimestamp == 0f))
                    {
                        BeatThisFrame = true;
                        LastBeatIndex = CurrentSequenceIndex;
                        LastBeatTimestamp = FeedbackTime;

                        for (int i = 0; i < Timing.Sequence.SequenceTracks.Count; i++)
                        {
                            if (Timing.Sequence.QuantizedSequence[i].Line[CurrentSequenceIndex].ID == Timing.TrackID)
                            {
                                CustomPlayFeedback(position, feedbacksIntensity);
                            }
                        }
                        CurrentSequenceIndex++;
                    }
                    yield return null;
                }
            }
            else
            {
                while (FeedbackTime - timeStartedAt < Timing.Sequence.Length)
                {
                    foreach (MMSequenceNote item in Timing.Sequence.OriginalSequence.Line)
                    {
                        if ((item.ID == Timing.TrackID) && (item.Timestamp >= lastFrame) && (item.Timestamp <= FeedbackTime - timeStartedAt))
                        {
                            CustomPlayFeedback(position, feedbacksIntensity);
                        }
                    }
                    lastFrame = FeedbackTime - timeStartedAt;
                    yield return null;
                }
            }
        }

        /// <summary>
        /// Use this method to change this feedback's sequence at runtime
        /// </summary>
        /// <param name="newSequence"></param>
        
        public virtual void SetSequence(MMSequence newSequence)
        {
            Timing.Sequence = newSequence;
            if (Timing.Sequence != null)
            {
                for (int i = 0; i < Timing.Sequence.SequenceTracks.Count; i++)
                {
                    if (Timing.Sequence.SequenceTracks[i].ID == Timing.TrackID)
                    {
                        _sequenceTrackID = i;
                    }
                }
            }
        }

        #endregion Sequence

        #region Controls

        /// <summary>
        /// Stops all feedbacks from playing. Will stop repeating feedbacks, and call custom stop implementations
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        public virtual void Stop(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (_playCoroutine != null) { Owner.StopCoroutine(_playCoroutine); }
            if (_infinitePlayCoroutine != null) { Owner.StopCoroutine(_infinitePlayCoroutine); }
            if (_repeatedPlayCoroutine != null) { Owner.StopCoroutine(_repeatedPlayCoroutine); }            
            if (_sequenceCoroutine != null) { Owner.StopCoroutine(_sequenceCoroutine);  }

            _lastPlayTimestamp = -1f;
            _playsLeft = Timing.NumberOfRepeats + 1;
            if (Timing.InterruptsOnStop)
            {
                CustomStopFeedback(position, feedbacksIntensity);    
            }
        }

        /// <summary>
        /// Called when skipping to the end of MMF_Player, calls custom Skip on all feedbacks
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        public virtual void SkipToTheEnd(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            CustomSkipToTheEnd(position, feedbacksIntensity);
        }

        /// <summary>
        /// Calls this feedback's custom reset 
        /// </summary>
        public virtual void ResetFeedback()
        {
            _playsLeft = Timing.NumberOfRepeats + 1;
            CustomReset();
        }

        #endregion

        #region Time
        
        /// <summary>
        /// Use this method to specify a new delay between repeats at runtime
        /// </summary>
        /// <param name="delay"></param>
        public virtual void SetDelayBetweenRepeats(float delay)
        {
            Timing.DelayBetweenRepeats = delay;
        }

        /// <summary>
        /// Use this method to specify a new initial delay at runtime
        /// </summary>
        /// <param name="delay"></param>
        public virtual void SetInitialDelay(float delay)
        {
            Timing.InitialDelay = delay;
        }

        /// <summary>
        /// Returns the t value at which to evaluate a curve at the end of this feedback's play time
        /// </summary>
        protected virtual float FinalNormalizedTime
        {
            get
            {
                return NormalPlayDirection ? 1f : 0f;
            }
        }

        /// <summary>
        /// Applies the host MMFeedbacks' time multiplier to this feedback
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        protected virtual float ApplyTimeMultiplier(float duration)
        {
            if (Owner == null)
            {
                return 0f;
            }
            return Owner.ApplyTimeMultiplier(duration);
        }
        
        #endregion Time

        #region Direction
        
        /// <summary>
        /// Returns a new value of the normalized time based on the current play direction of this feedback
        /// </summary>
        /// <param name="normalizedTime"></param>
        /// <returns></returns>
        protected virtual float ApplyDirection(float normalizedTime)
        {
            return NormalPlayDirection ? normalizedTime : 1 - normalizedTime;
        }

        /// <summary>
        /// Returns true if this feedback should play normally, or false if it should play in rewind
        /// </summary>
        public virtual bool NormalPlayDirection
        {
            get
            {
                switch (Timing.PlayDirection)
                {
                    case MMFeedbackTiming.PlayDirections.FollowMMFeedbacksDirection:
                        return (Owner.Direction == MMF_Player.Directions.TopToBottom);
                    case MMFeedbackTiming.PlayDirections.AlwaysNormal:
                        return true;
                    case MMFeedbackTiming.PlayDirections.AlwaysRewind:
                        return false;
                    case MMFeedbackTiming.PlayDirections.OppositeMMFeedbacksDirection:
                        return !(Owner.Direction == MMF_Player.Directions.TopToBottom);
                }
                return true;
            }
        }

        /// <summary>
        /// Returns true if this feedback should play in the current parent MMFeedbacks direction, according to its MMFeedbacksDirectionCondition setting
        /// </summary>
        public virtual bool ShouldPlayInThisSequenceDirection
        {
            get
            {
                switch (Timing.MMFeedbacksDirectionCondition)
                {
                    case MMFeedbackTiming.MMFeedbacksDirectionConditions.Always:
                        return true;
                    case MMFeedbackTiming.MMFeedbacksDirectionConditions.OnlyWhenForwards:
                        return (Owner.Direction == MMF_Player.Directions.TopToBottom);
                    case MMFeedbackTiming.MMFeedbacksDirectionConditions.OnlyWhenBackwards:
                        return (Owner.Direction == MMF_Player.Directions.BottomToTop);
                }
                return true;
            }
        }
        
        #endregion Direction

        #region Overrides 
        
        /// <summary>
        /// This method describes all custom initialization processes the feedback requires, in addition to the main Initialization method
        /// </summary>
        /// <param name="owner"></param>
        protected virtual void CustomInitialization(MMF_Player owner) { }

        /// <summary>
        /// This method describes what happens when the feedback gets played
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected abstract void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f);

        /// <summary>
        /// This method describes what happens when the feedback gets stopped
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected virtual void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1.0f) { }

        /// <summary>
        /// This method describes what happens when the feedback gets skipped to the end
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected virtual void CustomSkipToTheEnd(Vector3 position, float feedbacksIntensity = 1.0f) { }

        /// <summary>
        /// This method describes what happens when the feedback gets reset
        /// </summary>
        protected virtual void CustomReset() { }

        /// <summary>
        /// Use this method to initialize any custom attributes you may have
        /// </summary>
        public virtual void InitializeCustomAttributes() { }
        
        #endregion Overrides
        
        #region Event functions
        
        /// <summary>
        /// Triggered when a change happens in the inspector
        /// </summary>
        public virtual void OnValidate()
        {
            InitializeCustomAttributes();
        }

        /// <summary>
        /// Triggered when that feedback gets destroyed
        /// </summary>
        public virtual void OnDestroy()
        {
            
        }

        #endregion

    }    
}

