using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MoreMountains.Feedbacks
{
    [AddComponentMenu("More Mountains/Feedbacks/MMF Player")]
    [DisallowMultipleComponent] 
    public class MMF_Player : MMFeedbacks
    {
        #region PROPERTIES
        
        [SerializeReference]
        public List<MMF_Feedback> FeedbacksList;
        
        public override float TotalDuration
        {
            get
            {
                float total = 0f;
                if (FeedbacksList == null)
                {
                    return InitialDelay;
                }
                foreach (MMF_Feedback feedback in FeedbacksList)
                {
                    if ((feedback != null) && (feedback.Active))
                    {
                        if (total < feedback.TotalDuration)
                        {
                            total = feedback.TotalDuration;    
                        }
                    }
                }
                return InitialDelay + total;
            }
        }

        public bool KeepPlayModeChanges = false;
        [Tooltip("if this is true, the inspector won't refresh while the feedback plays, this saves on performance but feedback inspectors' progress bars for example won't look as smooth")]
        public bool PerformanceMode = false;

        public bool SkippingToTheEnd { get; protected set; }
        
        protected Type _t;
        
        #endregion
        
        #region INITIALIZATION

         /// <summary>
        /// On Awake we initialize our feedbacks if we're in auto mode
        /// </summary>
        protected override void Awake()
        {
            // if our MMFeedbacks is in AutoPlayOnEnable mode, we add a little helper to it that will re-enable it if needed if the parent game object gets turned off and on again
            if (AutoPlayOnEnable)
            {
                MMF_PlayerEnabler playerEnabler = GetComponent<MMF_PlayerEnabler>(); 
                if (playerEnabler == null)
                {
                    playerEnabler = this.gameObject.AddComponent<MMF_PlayerEnabler>();
                }
                playerEnabler.TargetMmfPlayer = this; 
            }
            
            if ((InitializationMode == InitializationModes.Awake) && (Application.isPlaying))
            {
                Initialization();
            }
            CheckForLoops();
        }

        /// <summary>
        /// On Start we initialize our feedbacks if we're in auto mode
        /// </summary>
        protected override void Start()
        {
            if ((InitializationMode == InitializationModes.Start) && (Application.isPlaying))
            {
                Initialization();
            }
            if (AutoPlayOnStart && Application.isPlaying)
            {
                PlayFeedbacks();
            }
            CheckForLoops();
        }

        protected virtual void InitializeList()
        {
            if (FeedbacksList == null)
            {
                FeedbacksList = new List<MMF_Feedback>();
            }
        }

        /// <summary>
        /// On Enable we initialize our feedbacks if we're in auto mode
        /// </summary>
        protected override void OnEnable()
        {
            if (AutoPlayOnEnable && Application.isPlaying)
            {
                PlayFeedbacks();
            }
            foreach (MMF_Feedback feedback in FeedbacksList)
            {
                feedback.CacheRequiresSetup();
            }
        }

        /// <summary>
        /// A public method to initialize the feedback, specifying an owner that will be used as the reference for position and hierarchy by feedbacks
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="feedbacksOwner"></param>
        public override void Initialization()
        {
            SkippingToTheEnd = false;
            IsPlaying = false;
            _lastStartAt = -float.MaxValue;

            int count = FeedbacksList.Count;
            for (int i = 0; i < count; i++)
            {
                if (FeedbacksList[i] != null)
                {
                    FeedbacksList[i].Initialization(this);
                }                
            }
        }

        #endregion

        #region PLAY
        
        /// <summary>
        /// Plays all feedbacks using the MMFeedbacks' position as reference, and no attenuation
        /// </summary>
        public override void PlayFeedbacks()
        {
            PlayFeedbacksInternal(this.transform.position, FeedbacksIntensity);
        }
        
        /// <summary>
        /// Plays all feedbacks, specifying a position and intensity. The position may be used by each Feedback and taken into account to spark a particle or play a sound for example.
        /// The feedbacks intensity is a factor that can be used by each Feedback to lower its intensity, usually you'll want to define that attenuation based on time or distance (using a lower 
        /// intensity value for feedbacks happening further away from the Player).
        /// Additionally you can force the feedback to play in reverse, ignoring its current condition
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksOwner"></param>
        /// <param name="feedbacksIntensity"></param>
        public override void PlayFeedbacks(Vector3 position, float feedbacksIntensity = 1.0f, bool forceRevert = false)
        {
            PlayFeedbacksInternal(position, feedbacksIntensity, forceRevert);
        }

        /// <summary>
        /// Plays all feedbacks using the MMFeedbacks' position as reference, and no attenuation, and in reverse (from bottom to top)
        /// </summary>
        public override void PlayFeedbacksInReverse()
        {
            PlayFeedbacksInternal(this.transform.position, FeedbacksIntensity, true);
        }

        /// <summary>
        /// Plays all feedbacks using the MMFeedbacks' position as reference, and no attenuation, and in reverse (from bottom to top)
        /// </summary>
        public override void PlayFeedbacksInReverse(Vector3 position, float feedbacksIntensity = 1.0f, bool forceRevert = false)
        {
            PlayFeedbacksInternal(position, feedbacksIntensity, forceRevert);
        }

        /// <summary>
        /// Plays all feedbacks in the sequence, but only if this MMFeedbacks is playing in reverse order
        /// </summary>
        public override void PlayFeedbacksOnlyIfReversed()
        {
            
            if ( (Direction == Directions.BottomToTop && !ShouldRevertOnNextPlay)
                 || ((Direction == Directions.TopToBottom) && ShouldRevertOnNextPlay) )
            {
                PlayFeedbacks();
            }
        }
        
        /// <summary>
        /// Plays all feedbacks in the sequence, but only if this MMFeedbacks is playing in reverse order
        /// </summary>
        public override void PlayFeedbacksOnlyIfReversed(Vector3 position, float feedbacksIntensity = 1.0f, bool forceRevert = false)
        {
            
            if ( (Direction == Directions.BottomToTop && !ShouldRevertOnNextPlay)
                 || ((Direction == Directions.TopToBottom) && ShouldRevertOnNextPlay) )
            {
                PlayFeedbacks(position, feedbacksIntensity, forceRevert);
            }
        }
        
        /// <summary>
        /// Plays all feedbacks in the sequence, but only if this MMFeedbacks is playing in normal order
        /// </summary>
        public override void PlayFeedbacksOnlyIfNormalDirection()
        {
            if (Direction == Directions.TopToBottom)
            {
                PlayFeedbacks();
            }
        }
        
        /// <summary>
        /// Plays all feedbacks in the sequence, but only if this MMFeedbacks is playing in normal order
        /// </summary>
        public override void PlayFeedbacksOnlyIfNormalDirection(Vector3 position, float feedbacksIntensity = 1.0f, bool forceRevert = false)
        {
            if (Direction == Directions.TopToBottom)
            {
                PlayFeedbacks(position, feedbacksIntensity, forceRevert);
            }
        }

        /// <summary>
        /// A public coroutine you can call externally when you want to yield in a coroutine of yours until the MMFeedbacks has stopped playing
        /// typically : yield return myFeedback.PlayFeedbacksCoroutine(this.transform.position, 1.0f, false);
        /// </summary>
        /// <param name="position">The position at which the MMFeedbacks should play</param>
        /// <param name="feedbacksIntensity">The intensity of the feedback</param>
        /// <param name="forceRevert">Whether or not the MMFeedbacks should play in reverse or not</param>
        /// <returns></returns>
        public override IEnumerator PlayFeedbacksCoroutine(Vector3 position, float feedbacksIntensity = 1.0f, bool forceRevert = false)
        {
            PlayFeedbacks(position, feedbacksIntensity, forceRevert);
            while (IsPlaying)
            {
                yield return null;    
            }
        }

        #endregion

        #region SEQUENCE

        /// <summary>
        /// An internal method used to play feedbacks, shouldn't be called externally
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void PlayFeedbacksInternal(Vector3 position, float feedbacksIntensity, bool forceRevert = false)
        {
            if (IsPlaying && !CanPlayWhileAlreadyPlaying)
            {
                return;
            }

            if (!EvaluateChance())
            {
                return;
            }

            // if we have a cooldown we prevent execution if needed
            if (CooldownDuration > 0f)
            {
                if (Time.unscaledTime - _lastStartAt < CooldownDuration)
                {
                    return;
                }
            }
            
            SkippingToTheEnd = false;

            // if all MMFeedbacks are disabled globally, we stop and don't play
            if (!GlobalMMFeedbacksActive)
            {
                return;
            }

            if (!this.gameObject.activeInHierarchy)
            {
                return;
            }
            
            if (ShouldRevertOnNextPlay)
            {
                Revert();
                ShouldRevertOnNextPlay = false;
            }

            if (forceRevert)
            {
                Direction = (Direction == Directions.BottomToTop) ? Directions.TopToBottom : Directions.BottomToTop;
            }
            
            ResetFeedbacks();
            this.enabled = true;
            IsPlaying = true;
            _startTime = Time.unscaledTime;
            _lastStartAt = _startTime;
            _totalDuration = TotalDuration;
            
            if (InitialDelay > 0f)
            {
                StartCoroutine(HandleInitialDelayCo(position, feedbacksIntensity, forceRevert));
            }
            else
            {
                PreparePlay(position, feedbacksIntensity, forceRevert);
            }
        }

        protected override void PreparePlay(Vector3 position, float feedbacksIntensity, bool forceRevert = false)
        {
            Events.TriggerOnPlay(this);

            _holdingMax = 0f;

            // test if a pause or holding pause is found
            _pauseFound = false;
            int count = FeedbacksList.Count;
            for (int i = 0; i < count; i++)
            {
                if (FeedbacksList[i] != null)
                {
                    if ((FeedbacksList[i].Pause != null) && (FeedbacksList[i].Active) && (FeedbacksList[i].ShouldPlayInThisSequenceDirection))
                    {
                        _pauseFound = true;
                    }
                    if ((FeedbacksList[i].HoldingPause == true) && (FeedbacksList[i].Active) && (FeedbacksList[i].ShouldPlayInThisSequenceDirection))
                    {
                        _pauseFound = true;
                    }    
                }
            }

            if (!_pauseFound)
            {
                PlayAllFeedbacks(position, feedbacksIntensity, forceRevert);
            }
            else
            {
                // if at least one pause was found
                StartCoroutine(PausedFeedbacksCo(position, feedbacksIntensity));
            }
        }

        protected override void PlayAllFeedbacks(Vector3 position, float feedbacksIntensity, bool forceRevert = false)
        {
            // if no pause was found, we just play all feedbacks at once
            int count = FeedbacksList.Count;
            for (int i = 0; i < count; i++)
            {
                if (FeedbackCanPlay(FeedbacksList[i]))
                {
                    FeedbacksList[i].Play(position, feedbacksIntensity);
                }
            }
        }

        protected override IEnumerator HandleInitialDelayCo(Vector3 position, float feedbacksIntensity, bool forceRevert = false)
        {
            IsPlaying = true;
            yield return MMFeedbacksCoroutine.WaitFor(InitialDelay);
            PreparePlay(position, feedbacksIntensity, forceRevert);
        }
        
        protected override void Update()
        {
            if (_shouldStop)
            {
                if (HasFeedbackStillPlaying())
                {
                    return;
                }
                IsPlaying = false;
                Events.TriggerOnComplete(this);
                ApplyAutoRevert();
                this.enabled = false;
                _shouldStop = false;
            }
            if (IsPlaying)
            {
                if (!_pauseFound)
                {
                    if (Time.unscaledTime - _startTime > _totalDuration)
                    {
                        _shouldStop = true;
                    }    
                }
            }
            else
            {
                this.enabled = false;
            }
        }

        /// <summary>
        /// A coroutine used to handle the sequence of feedbacks if pauses are involved
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        /// <returns></returns>
        protected override IEnumerator PausedFeedbacksCo(Vector3 position, float feedbacksIntensity)
        {
            IsPlaying = true;

            int i = (Direction == Directions.TopToBottom) ? 0 : FeedbacksList.Count-1;

            int count = FeedbacksList.Count;
            while ((i >= 0) && (i < count))
            {
                if (!IsPlaying)
                {
                    yield break;
                }

                if (FeedbacksList[i] == null)
                {
                    yield break;
                }
                
                if (((FeedbacksList[i].Active) && (FeedbacksList[i].ScriptDrivenPause)) || InScriptDrivenPause)
                {
                    InScriptDrivenPause = true;

                    bool inAutoResume = (FeedbacksList[i].ScriptDrivenPauseAutoResume > 0f); 
                    float scriptDrivenPauseStartedAt = Time.unscaledTime;
                    float autoResumeDuration = FeedbacksList[i].ScriptDrivenPauseAutoResume;
                    
                    while (InScriptDrivenPause)
                    {
                        if (inAutoResume && (Time.unscaledTime - scriptDrivenPauseStartedAt > autoResumeDuration))
                        {
                            ResumeFeedbacks();
                        }
                        yield return null;
                    } 
                }

                // handles holding pauses
                if ((FeedbacksList[i].Active)
                    && ((FeedbacksList[i].HoldingPause == true) || (FeedbacksList[i].LooperPause == true))
                    && (FeedbacksList[i].ShouldPlayInThisSequenceDirection))
                {
                    Events.TriggerOnPause(this);
                    // we stay here until all previous feedbacks have finished
                    while (Time.unscaledTime - _lastStartAt < _holdingMax)
                    {
                        yield return null;
                    }
                    _holdingMax = 0f;
                    _lastStartAt = Time.unscaledTime;
                }

                // plays the feedback
                if (FeedbackCanPlay(FeedbacksList[i]))
                {
                    FeedbacksList[i].Play(position, feedbacksIntensity);
                }

                // Handles pause
                if ((FeedbacksList[i].Pause != null) && (FeedbacksList[i].Active) && (FeedbacksList[i].ShouldPlayInThisSequenceDirection))
                {
                    bool shouldPause = true;
                    if (FeedbacksList[i].Chance < 100)
                    {
                        float random = Random.Range(0f, 100f);
                        if (random > FeedbacksList[i].Chance)
                        {
                            shouldPause = false;
                        }
                    }

                    if (shouldPause)
                    {
                        yield return FeedbacksList[i].Pause;
                        Events.TriggerOnResume(this);
                        _lastStartAt = Time.unscaledTime;
                        _holdingMax = 0f;
                    }
                }

                // updates holding max
                if (FeedbacksList[i].Active)
                {
                    if ((FeedbacksList[i].Pause == null) && (FeedbacksList[i].ShouldPlayInThisSequenceDirection) && (!FeedbacksList[i].Timing.ExcludeFromHoldingPauses))
                    {
                        float feedbackDuration = FeedbacksList[i].TotalDuration;
                        _holdingMax = Mathf.Max(feedbackDuration, _holdingMax);
                    }
                }

                // handles looper
                if ((FeedbacksList[i].LooperPause == true)
                    && (FeedbacksList[i].Active)
                    && (FeedbacksList[i].ShouldPlayInThisSequenceDirection)
                    && (((FeedbacksList[i] as MMF_Looper).NumberOfLoopsLeft > 0) || (FeedbacksList[i] as MMF_Looper).InInfiniteLoop))
                {
                    // we determine the index we should start again at
                    bool loopAtLastPause = (FeedbacksList[i] as MMF_Looper).LoopAtLastPause;
                    bool loopAtLastLoopStart = (FeedbacksList[i] as MMF_Looper).LoopAtLastLoopStart;
                    
                    int newi = 0;

                    int j = (Direction == Directions.TopToBottom) ? i - 1 : i + 1;

                    int listCount = FeedbacksList.Count;
                    while ((j >= 0) && (j <= listCount))
                    {
                        // if we're at the start
                        if (j == 0)
                        {
                            newi = j - 1;
                            break;
                        }
                        if (j == listCount)
                        {
                            newi = j ;
                            break;
                        }
                        // if we've found a pause
                        if ((FeedbacksList[j].Pause != null)
                            && (FeedbacksList[j].FeedbackDuration > 0f)
                            && loopAtLastPause && (FeedbacksList[j].Active))
                        {
                            newi = j;
                            break;
                        }
                        // if we've found a looper start
                        if ((FeedbacksList[j].LooperStart == true)
                            && loopAtLastLoopStart
                            && (FeedbacksList[j].Active))
                        {
                            newi = j;
                            break;
                        }

                        j += (Direction == Directions.TopToBottom) ? -1 : 1;
                    }
                    i = newi;
                }
                i += (Direction == Directions.TopToBottom) ? 1 : -1;
            }
            float unscaledTimeAtEnd = Time.unscaledTime;
            while (Time.unscaledTime - unscaledTimeAtEnd < _holdingMax)
            {
                yield return null;
            }
            while (HasFeedbackStillPlaying())
            {
                yield return null;
            }
            IsPlaying = false;
            Events.TriggerOnComplete(this);
            ApplyAutoRevert();
        }

        protected virtual IEnumerator SkipToTheEndCo()
        {
            SkippingToTheEnd = true;
            Events.TriggerOnSkip(this);
            int count = FeedbacksList.Count;
            for (int i = 0; i < count; i++)
            {
                if ((FeedbacksList[i] != null) && (FeedbacksList[i].Active))
                {
                    FeedbacksList[i].SkipToTheEnd(this.transform.position);    
                }
            }
            yield return null;
            yield return null;
            SkippingToTheEnd = false;
            StopFeedbacks();
        }

        #endregion

        #region STOP

        /// <summary>
        /// Stops all further feedbacks from playing, without stopping individual feedbacks 
        /// </summary>
        public override void StopFeedbacks()
        {
            StopFeedbacks(true);
        }

        /// <summary>
        /// Stops all feedbacks from playing, with an option to also stop individual feedbacks
        /// </summary>
        public override void StopFeedbacks(bool stopAllFeedbacks = true)
        {
            StopFeedbacks(this.transform.position, 1.0f, stopAllFeedbacks);
        }

        /// <summary>
        /// Stops all feedbacks from playing, specifying a position and intensity that can be used by the Feedbacks 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        public override void StopFeedbacks(Vector3 position, float feedbacksIntensity = 1.0f, bool stopAllFeedbacks = true)
        {
            if (stopAllFeedbacks)
            {
                int count = FeedbacksList.Count;
                for (int i = 0; i < count; i++)
                {
                    FeedbacksList[i].Stop(position, feedbacksIntensity);
                }    
            }
            IsPlaying = false;
            StopAllCoroutines();
        }
        
        #endregion 

        #region CONTROLS

        /// <summary>
        /// Calls each feedback's Reset method if they've defined one. An example of that can be resetting the initial color of a flickering renderer.
        /// </summary>
        public override void ResetFeedbacks()
        {
            int count = FeedbacksList.Count;
            for (int i = 0; i < count; i++)
            {
                if ((FeedbacksList[i] != null) && (FeedbacksList[i].Active))
                {
                    FeedbacksList[i].ResetFeedback();    
                }
            }
            IsPlaying = false;
        }

        /// <summary>
        /// Changes the direction of this MMFeedbacks
        /// </summary>
        public override void Revert()
        {
            Events.TriggerOnRevert(this);
            Direction = (Direction == Directions.BottomToTop) ? Directions.TopToBottom : Directions.BottomToTop;
        }

        /// <summary>
        /// Pauses execution of a sequence, which can then be resumed by calling ResumeFeedbacks()
        /// </summary>
        public override void PauseFeedbacks()
        {
            Events.TriggerOnPause(this);
            InScriptDrivenPause = true;
        }

        /// <summary>
        /// Pauses execution of a sequence, which can then be resumed by calling ResumeFeedbacks()
        /// </summary>
        public virtual void SkipToTheEnd()
        {
            StartCoroutine(SkipToTheEndCo());
        }

        /// <summary>
        /// Resumes execution of a sequence if a script driven pause is in progress
        /// </summary>
        public override void ResumeFeedbacks()
        {
            Events.TriggerOnResume(this);
            InScriptDrivenPause = false;
        }

        #endregion
        
        #region MODIFICATION

        /// <summary>
        /// Adds the specified MMF_Feedback to the player
        /// </summary>
        /// <param name="newFeedback"></param>
        public virtual void AddFeedback(MMF_Feedback newFeedback)
        {
            InitializeList();
            newFeedback.Owner = this;
            newFeedback.UniqueID = Guid.NewGuid().GetHashCode();
            FeedbacksList.Add(newFeedback);
            newFeedback.CacheRequiresSetup();
            newFeedback.InitializeCustomAttributes();
        }
        
        /// <summary>
        /// Adds a feedback of the specified type to the player
        /// </summary>
        /// <param name="feedbackType"></param>
        /// <returns></returns>
        public new MMF_Feedback AddFeedback(System.Type feedbackType)
        {
            InitializeList();
            MMF_Feedback newFeedback = (MMF_Feedback)Activator.CreateInstance(feedbackType);
            newFeedback.Label = FeedbackPathAttribute.GetFeedbackDefaultName(feedbackType);
            newFeedback.Owner = this;
            newFeedback.UniqueID = Guid.NewGuid().GetHashCode();
            FeedbacksList.Add(newFeedback);
            newFeedback.InitializeCustomAttributes();
            newFeedback.CacheRequiresSetup();
            return newFeedback;
        }
        
        /// <summary>
        /// Removes the feedback at the specified index
        /// </summary>
        /// <param name="id"></param>
        public override void RemoveFeedback(int id)
        {
	        if (FeedbacksList.Count < id)
	        {
		        return;
	        }
            FeedbacksList.RemoveAt(id);
        }
        
        #endregion MODIFICATION

        #region HELPERS
        
        /// <summary>
        /// Returns true if feedbacks are still playing
        /// </summary>
        /// <returns></returns>
        public override bool HasFeedbackStillPlaying()
        {
            int count = FeedbacksList.Count;
            for (int i = 0; i < count; i++)
            {
                if (FeedbacksList[i].IsPlaying)
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Checks whether or not this MMFeedbacks contains one or more looper feedbacks
        /// </summary>
        protected override void CheckForLoops()
        {
            ContainsLoop = false;
            int count = FeedbacksList.Count;
            for (int i = 0; i < count; i++)
            {
                if (FeedbacksList[i] != null)
                {
                    if (FeedbacksList[i].LooperPause && FeedbacksList[i].Active)
                    {
                        ContainsLoop = true;
                        return;
                    }
                }                
            }
        }
        
        /// <summary>
        /// This will return true if the conditions defined in the specified feedback's Timing section allow it to play in the current play direction of this MMFeedbacks
        /// </summary>
        /// <param name="feedback"></param>
        /// <returns></returns>
        protected bool FeedbackCanPlay(MMF_Feedback feedback)
        {
            if (feedback.Timing.MMFeedbacksDirectionCondition == MMFeedbackTiming.MMFeedbacksDirectionConditions.Always)
            {
                return true;
            }
            else if (((Direction == Directions.TopToBottom) && (feedback.Timing.MMFeedbacksDirectionCondition == MMFeedbackTiming.MMFeedbacksDirectionConditions.OnlyWhenForwards))
                     || ((Direction == Directions.BottomToTop) && (feedback.Timing.MMFeedbacksDirectionCondition == MMFeedbackTiming.MMFeedbacksDirectionConditions.OnlyWhenBackwards)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Readies the MMFeedbacks to revert direction on the next play
        /// </summary>
        protected override void ApplyAutoRevert()
        {
            if (AutoChangeDirectionOnEnd)
            {
                ShouldRevertOnNextPlay = true;
            }
        }
        
        /// <summary>
        /// Applies this feedback's time multiplier to a duration (in seconds)
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public override float ApplyTimeMultiplier(float duration)
        {
            return duration * DurationMultiplier;
        }

        /// <summary>
        /// Lets you destroy objects from feedbacks
        /// </summary>
        /// <param name="gameObjectToDestroy"></param>
        public virtual void ProxyDestroy(GameObject gameObjectToDestroy)
        {
            Destroy(gameObjectToDestroy);
        }
        
        /// <summary>
        /// Lets you destroy objects after a delay from feedbacks
        /// </summary>
        /// <param name="gameObjectToDestroy"></param>
        /// <param name="delay"></param>
        public virtual void ProxyDestroy(GameObject gameObjectToDestroy, float delay)
        {
            Destroy(gameObjectToDestroy, delay);
        }

        /// <summary>
        /// Lets you DestroyImmediate objects from feedbacks
        /// </summary>
        /// <param name="gameObjectToDestroy"></param>
        public virtual void ProxyDestroyImmediate(GameObject gameObjectToDestroy)
        {
            DestroyImmediate(gameObjectToDestroy);
        }
        
        #endregion

        #region ACCESS

        /// <summary>
        /// Returns the first feedback of the searched type on this MMF_Player
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T GetFeedbackOfType<T>() where T:MMF_Feedback
        {
            _t = typeof(T);
            foreach (MMF_Feedback feedback in FeedbacksList)
            {
                if (feedback.GetType() == _t)
                {
                    return (T)feedback;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a list of all the feedbacks of the searched type on this MMF_Player
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual List<T> GetFeedbacksOfType<T>() where T:MMF_Feedback
        {
            _t = typeof(T);
            List<T> list = new List<T>();
            foreach (MMF_Feedback feedback in FeedbacksList)
            {
                if (feedback.GetType() == _t)
                {
                    list.Add((T)feedback);
                }
            }
            return list;
        }

        /// <summary>
        /// Returns the first feedback of the searched type on this MMF_Player
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T GetFeedbackOfType<T>(string searchedLabel) where T:MMF_Feedback
        {
            _t = typeof(T);
            foreach (MMF_Feedback feedback in FeedbacksList)
            {
                if ((feedback.GetType() == _t) && (feedback.Label == searchedLabel))
                {
                    return (T)feedback;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a list of all the feedbacks of the searched type on this MMF_Player
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual List<T> GetFeedbacksOfType<T>(string searchedLabel) where T:MMF_Feedback
        {
            _t = typeof(T);
            List<T> list = new List<T>();
            foreach (MMF_Feedback feedback in FeedbacksList)
            {
                if ((feedback.GetType() == _t) && (feedback.Label == searchedLabel))
                {
                    list.Add((T)feedback);
                }
            }
            return list;
        }

        #endregion
        
        #region EVENTS

        /// <summary>
        /// On Disable we stop all feedbacks
        /// </summary>
        protected override void OnDisable()
        {
            /*if (IsPlaying)
            {
                StopFeedbacks();
                StopAllCoroutines();
            }*/
        }

        /// <summary>
        /// On validate, we make sure our DurationMultiplier remains positive
        /// </summary>
        protected override void OnValidate()
        {
            RefreshCache();
        }

        /// <summary>
        /// Refreshes cached feedbacks
        /// </summary>
        public virtual void RefreshCache()
        {
            if (FeedbacksList == null)
            {
                return;
            }
            
            DurationMultiplier = Mathf.Clamp(DurationMultiplier, 0f, Single.MaxValue);
            
            foreach (MMF_Feedback feedback in FeedbacksList)
            {
                feedback.CacheRequiresSetup();
                feedback.OnValidate();
            }
        }

        /// <summary>
        /// On Destroy, removes all feedbacks from this MMFeedbacks to avoid any leftovers
        /// </summary>
        protected override void OnDestroy()
        {
            IsPlaying = false;
            
            foreach (MMF_Feedback feedback in FeedbacksList)
            {
                feedback.OnDestroy();
            }
        }

        #endregion EVENTS
    }    
}

