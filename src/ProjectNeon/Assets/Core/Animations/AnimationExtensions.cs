
using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public static class AnimationExtensions
{
    public static void DOPunchScaleStandard(this Transform t, Vector3 originalScale)
    {
        t.localScale = originalScale;
        t.DOPunchScale(new Vector3(1.28f, 1.28f, 1.28f), 1f, 1);
    }

    public static IEnumerator PlayAnimationUntilFinished(this Animator animator, string animationName, Action<float> onFinished, int layer = 0, float maxTime = 5f)
    {
        if (animator.parameters.Any(x => x.name == animationName))
        {
            var elapsed = 0f; 
            animator.SetTrigger(animationName);
            bool hasStartedAnimated = false;
            while (elapsed < maxTime)
            {
                yield return new WaitForSeconds(0.1f);
                elapsed += 0.1f;
                var isPlayingAnimation = animator.GetCurrentAnimatorStateInfo(layer).IsName(animationName);
                if (isPlayingAnimation && !hasStartedAnimated)
                    hasStartedAnimated = true;
                if (!isPlayingAnimation && hasStartedAnimated)
                    break;
            }
            if (!hasStartedAnimated)
                Log.Error($"Animation: {animationName} never started");
            onFinished(elapsed);
        }
        else
        {
            Log.Error($"Animation: {animationName} not found");
            onFinished(0);
        }
    }
}
