using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace MoreMountains.Tools
{
    /// <summary>
    /// The formulas described here are (loosely) based on Robert Penner's easing equations http://robertpenner.com/easing/
    /// I recommend reading this blog post if you're interested in the subject : http://blog.moagrius.com/actionscript/jsas-understanding-easing/
    /// </summary>

    public class MMTween : MonoBehaviour
    {
        /// <summary>
        /// A list of all the possible curves you can tween a value along
        /// </summary>
        public enum MMTweenCurve
        {
            LinearTween,        
            EaseInQuadratic,    EaseOutQuadratic,   EaseInOutQuadratic,
            EaseInCubic,        EaseOutCubic,       EaseInOutCubic,
            EaseInQuartic,      EaseOutQuartic,     EaseInOutQuartic,
            EaseInQuintic,      EaseOutQuintic,     EaseInOutQuintic,
            EaseInSinusoidal,   EaseOutSinusoidal,  EaseInOutSinusoidal,
            EaseInBounce,       EaseOutBounce,      EaseInOutBounce,
            EaseInOverhead,     EaseOutOverhead,    EaseInOutOverhead,
            EaseInExponential,  EaseOutExponential, EaseInOutExponential,
            EaseInElastic,      EaseOutElastic,     EaseInOutElastic,
            EaseInCircular,     EaseOutCircular,    EaseInOutCircular,
            AntiLinearTween,    AlmostIdentity
        }

        // Core methods ---------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Moves a value between a startValue and an endValue based on a currentTime, along the specified tween curve
        /// </summary>
        /// <param name="currentTime"></param>
        /// <param name="initialTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static float Tween(float currentTime, float initialTime, float endTime, float startValue, float endValue, MMTweenCurve curve)
        {
            currentTime = MMMaths.Remap(currentTime, initialTime, endTime, 0f, 1f);
            switch (curve)
            {
                case MMTweenCurve.LinearTween: currentTime = MMTweenDefinitions.Linear_Tween(currentTime); break;
                case MMTweenCurve.AntiLinearTween: currentTime = MMTweenDefinitions.LinearAnti_Tween(currentTime); break;

                case MMTweenCurve.EaseInQuadratic: currentTime = MMTweenDefinitions.EaseIn_Quadratic(currentTime); break;
                case MMTweenCurve.EaseOutQuadratic: currentTime = MMTweenDefinitions.EaseOut_Quadratic(currentTime); break;
                case MMTweenCurve.EaseInOutQuadratic: currentTime = MMTweenDefinitions.EaseInOut_Quadratic(currentTime); break;

                case MMTweenCurve.EaseInCubic: currentTime = MMTweenDefinitions.EaseIn_Cubic(currentTime); break;
                case MMTweenCurve.EaseOutCubic: currentTime = MMTweenDefinitions.EaseOut_Cubic(currentTime); break;
                case MMTweenCurve.EaseInOutCubic: currentTime = MMTweenDefinitions.EaseInOut_Cubic(currentTime); break;

                case MMTweenCurve.EaseInQuartic: currentTime = MMTweenDefinitions.EaseIn_Quartic(currentTime); break;
                case MMTweenCurve.EaseOutQuartic: currentTime = MMTweenDefinitions.EaseOut_Quartic(currentTime); break;
                case MMTweenCurve.EaseInOutQuartic: currentTime = MMTweenDefinitions.EaseInOut_Quartic(currentTime); break;

                case MMTweenCurve.EaseInQuintic: currentTime = MMTweenDefinitions.EaseIn_Quintic(currentTime); break;
                case MMTweenCurve.EaseOutQuintic: currentTime = MMTweenDefinitions.EaseOut_Quintic(currentTime); break;
                case MMTweenCurve.EaseInOutQuintic: currentTime = MMTweenDefinitions.EaseInOut_Quintic(currentTime); break;

                case MMTweenCurve.EaseInSinusoidal: currentTime = MMTweenDefinitions.EaseIn_Sinusoidal(currentTime); break;
                case MMTweenCurve.EaseOutSinusoidal: currentTime = MMTweenDefinitions.EaseOut_Sinusoidal(currentTime); break;
                case MMTweenCurve.EaseInOutSinusoidal: currentTime = MMTweenDefinitions.EaseInOut_Sinusoidal(currentTime); break;

                case MMTweenCurve.EaseInBounce: currentTime = MMTweenDefinitions.EaseIn_Bounce(currentTime); break;
                case MMTweenCurve.EaseOutBounce: currentTime = MMTweenDefinitions.EaseOut_Bounce(currentTime); break;
                case MMTweenCurve.EaseInOutBounce: currentTime = MMTweenDefinitions.EaseInOut_Bounce(currentTime); break;

                case MMTweenCurve.EaseInOverhead: currentTime = MMTweenDefinitions.EaseIn_Overhead(currentTime); break;
                case MMTweenCurve.EaseOutOverhead: currentTime = MMTweenDefinitions.EaseOut_Overhead(currentTime); break;
                case MMTweenCurve.EaseInOutOverhead: currentTime = MMTweenDefinitions.EaseInOut_Overhead(currentTime); break;

                case MMTweenCurve.EaseInExponential: currentTime = MMTweenDefinitions.EaseIn_Exponential(currentTime); break;
                case MMTweenCurve.EaseOutExponential: currentTime = MMTweenDefinitions.EaseOut_Exponential(currentTime); break;
                case MMTweenCurve.EaseInOutExponential: currentTime = MMTweenDefinitions.EaseInOut_Exponential(currentTime); break;

                case MMTweenCurve.EaseInElastic: currentTime = MMTweenDefinitions.EaseIn_Elastic(currentTime); break;
                case MMTweenCurve.EaseOutElastic: currentTime = MMTweenDefinitions.EaseOut_Elastic(currentTime); break;
                case MMTweenCurve.EaseInOutElastic: currentTime = MMTweenDefinitions.EaseInOut_Elastic(currentTime); break;

                case MMTweenCurve.EaseInCircular: currentTime = MMTweenDefinitions.EaseIn_Circular(currentTime); break;
                case MMTweenCurve.EaseOutCircular: currentTime = MMTweenDefinitions.EaseOut_Circular(currentTime); break;
                case MMTweenCurve.EaseInOutCircular: currentTime = MMTweenDefinitions.EaseInOut_Circular(currentTime); break;

                case MMTweenCurve.AlmostIdentity: currentTime = MMTweenDefinitions.AlmostIdentity(currentTime); break;

            }
            return startValue + currentTime * (endValue - startValue);
        }

        public static Vector2 Tween(float currentTime, float initialTime, float endTime, Vector2 startValue, Vector2 endValue, MMTweenCurve curve)
        {
            startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
            startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
            return startValue;
        }

        public static Vector3 Tween(float currentTime, float initialTime, float endTime, Vector3 startValue, Vector3 endValue, MMTweenCurve curve)
        {
            startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
            startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
            startValue.z = Tween(currentTime, initialTime, endTime, startValue.z, endValue.z, curve);
            return startValue;
        }

        public static Quaternion Tween(float currentTime, float initialTime, float endTime, Quaternion startValue, Quaternion endValue, MMTweenCurve curve)
        {
            float turningRate = Tween(currentTime, initialTime, endTime, 0f, 1f, curve);
            startValue = Quaternion.Slerp(startValue, endValue, turningRate);
            return startValue;
        }

        // Animation curve methods --------------------------------------------------------------------------------------------------------------

        public static float Tween(float currentTime, float initialTime, float endTime, float startValue, float endValue, AnimationCurve curve)
        {
            currentTime = MMMaths.Remap(currentTime, initialTime, endTime, 0f, 1f);
            currentTime = curve.Evaluate(currentTime);
            return startValue + currentTime * (endValue - startValue);
        }

        public static Vector2 Tween(float currentTime, float initialTime, float endTime, Vector2 startValue, Vector2 endValue, AnimationCurve curve)
        {
            startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
            startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
            return startValue;
        }

        public static Vector3 Tween(float currentTime, float initialTime, float endTime, Vector3 startValue, Vector3 endValue, AnimationCurve curve)
        {
            startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
            startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
            startValue.z = Tween(currentTime, initialTime, endTime, startValue.z, endValue.z, curve);
            return startValue;
        }

        public static Quaternion Tween(float currentTime, float initialTime, float endTime, Quaternion startValue, Quaternion endValue, AnimationCurve curve)
        {
            float turningRate = Tween(currentTime, initialTime, endTime, 0f, 1f, curve);
            startValue = Quaternion.Slerp(startValue, endValue, turningRate);
            return startValue;
        }

        // Tween type methods ------------------------------------------------------------------------------------------------------------------------

        public static float Tween(float currentTime, float initialTime, float endTime, float startValue, float endValue, MMTweenType tweenType)
        {
            if (tweenType.MMTweenDefinitionType == MMTweenDefinitionTypes.MMTween)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.MMTweenCurve);
            }
            if (tweenType.MMTweenDefinitionType == MMTweenDefinitionTypes.AnimationCurve)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
            }
            return 0f;
        }
        public static Vector2 Tween(float currentTime, float initialTime, float endTime, Vector2 startValue, Vector2 endValue, MMTweenType tweenType)
        {
            if (tweenType.MMTweenDefinitionType == MMTweenDefinitionTypes.MMTween)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.MMTweenCurve);
            }
            if (tweenType.MMTweenDefinitionType == MMTweenDefinitionTypes.AnimationCurve)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
            }
            return Vector2.zero;
        }
        public static Vector3 Tween(float currentTime, float initialTime, float endTime, Vector3 startValue, Vector3 endValue, MMTweenType tweenType)
        {
            if (tweenType.MMTweenDefinitionType == MMTweenDefinitionTypes.MMTween)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.MMTweenCurve);
            }
            if (tweenType.MMTweenDefinitionType == MMTweenDefinitionTypes.AnimationCurve)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
            }
            return Vector3.zero;
        }
        public static Quaternion Tween(float currentTime, float initialTime, float endTime, Quaternion startValue, Quaternion endValue, MMTweenType tweenType)
        {
            if (tweenType.MMTweenDefinitionType == MMTweenDefinitionTypes.MMTween)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.MMTweenCurve);
            }
            if (tweenType.MMTweenDefinitionType == MMTweenDefinitionTypes.AnimationCurve)
            {
                return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
            }
            return Quaternion.identity;
        }

        // MOVE METHODS ---------------------------------------------------------------------------------------------------------
        public static Coroutine MoveTransform(MonoBehaviour mono, Transform targetTransform, Vector3 origin, Vector3 destination, 
            WaitForSeconds delay, float delayDuration, float duration, MMTween.MMTweenCurve curve, bool ignoreTimescale = false)
        {
            return mono.StartCoroutine(MoveTransformCo(targetTransform, origin, destination, delay, delayDuration, duration, curve, ignoreTimescale));
        }

        public static Coroutine MoveRectTransform(MonoBehaviour mono, RectTransform targetTransform, Vector3 origin, Vector3 destination,
            WaitForSeconds delay, float delayDuration, float duration, MMTween.MMTweenCurve curve, bool ignoreTimescale = false)
        {
            return mono.StartCoroutine(MoveRectTransformCo(targetTransform, origin, destination, delay, delayDuration, duration, curve, ignoreTimescale));
        }

        public static Coroutine MoveTransform(MonoBehaviour mono, Transform targetTransform, Transform origin, Transform destination, WaitForSeconds delay, float delayDuration, float duration, 
            MMTween.MMTweenCurve curve, bool updatePosition = true, bool updateRotation = true, bool ignoreTimescale = false)
        {
            return mono.StartCoroutine(MoveTransformCo(targetTransform, origin, destination, delay, delayDuration, duration, curve, updatePosition, updateRotation, ignoreTimescale));
        }

        public static Coroutine RotateTransformAround(MonoBehaviour mono, Transform targetTransform, Transform center, Transform destination, float angle, WaitForSeconds delay, float delayDuration, 
            float duration, MMTween.MMTweenCurve curve, bool ignoreTimescale = false)
        {
            return mono.StartCoroutine(RotateTransformAroundCo(targetTransform, center, destination, angle, delay, delayDuration, duration, curve, ignoreTimescale));
        }

        protected static IEnumerator MoveRectTransformCo(RectTransform targetTransform, Vector3 origin, Vector3 destination, WaitForSeconds delay,
            float delayDuration, float duration, MMTween.MMTweenCurve curve, bool ignoreTimescale = false)
        {
            if (delayDuration > 0f)
            {
                yield return delay;
            }
            float timeLeft = duration;
            while (timeLeft > 0f)
            {
                targetTransform.localPosition = MMTween.Tween(duration - timeLeft, 0f, duration, origin, destination, curve);
                timeLeft -= ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
            targetTransform.localPosition = destination;
        }

        protected static IEnumerator MoveTransformCo(Transform targetTransform, Vector3 origin, Vector3 destination, WaitForSeconds delay, 
            float delayDuration, float duration, MMTween.MMTweenCurve curve, bool ignoreTimescale = false)
        {
            if (delayDuration > 0f)
            {
                yield return delay;
            }
            float timeLeft = duration;
            while (timeLeft > 0f)
            {
                targetTransform.transform.position = MMTween.Tween(duration - timeLeft, 0f, duration, origin, destination, curve);
                timeLeft -= ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
            targetTransform.transform.position = destination;
        }

        protected static IEnumerator MoveTransformCo(Transform targetTransform, Transform origin, Transform destination, WaitForSeconds delay, float delayDuration, float duration, 
            MMTween.MMTweenCurve curve, bool updatePosition = true, bool updateRotation = true, bool ignoreTimescale = false)
        {
            if (delayDuration > 0f)
            {
                yield return delay;
            }
            float timeLeft = duration;
            while (timeLeft > 0f)
            {
                if (updatePosition)
                {
                    targetTransform.transform.position = MMTween.Tween(duration - timeLeft, 0f, duration, origin.position, destination.position, curve);
                }
                if (updateRotation)
                {
                    targetTransform.transform.rotation = MMTween.Tween(duration - timeLeft, 0f, duration, origin.rotation, destination.rotation, curve);
                }
                timeLeft -= ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
            if (updatePosition) { targetTransform.transform.position = destination.position; }
            if (updateRotation) { targetTransform.transform.localEulerAngles = destination.localEulerAngles; }
        }

        protected static IEnumerator RotateTransformAroundCo(Transform targetTransform, Transform center, Transform destination, float angle, WaitForSeconds delay, float delayDuration, float duration, 
            MMTween.MMTweenCurve curve, bool ignoreTimescale = false)
        {
            if (delayDuration > 0f)
            {
                yield return delay;
            }

            Vector3 initialRotationPosition = targetTransform.transform.position;
            Quaternion initialRotationRotation = targetTransform.transform.rotation;

            float rate = 1f / duration;

            float timeSpent = 0f;
            while (timeSpent < duration)
            {

                float newAngle = MMTween.Tween(timeSpent, 0f, duration, 0f, angle, curve);

                targetTransform.transform.position = initialRotationPosition;
                initialRotationRotation = targetTransform.transform.rotation;
                targetTransform.RotateAround(center.transform.position, center.transform.up, newAngle);
                targetTransform.transform.rotation = initialRotationRotation;

                timeSpent += ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
            targetTransform.transform.position = destination.position;
        }
    }
}