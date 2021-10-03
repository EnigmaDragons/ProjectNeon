
using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public static class AnimationExtensions
{
    public static void DOPunchScaleStandard(this Transform t, Vector3 originalScale, float factor = 1.28f)
    {
        t.localScale = originalScale;
        t.DOPunchScale(new Vector3(factor, factor, factor), 1f, 1);
    }

    public static IEnumerator PlayAnimationUntilFinished(this Animator animator, string animationName, Action<float> onFinished, int layer = 0, float maxTime = 5f)
    {
        var isTrigger = animator.parameters.Any(x => x.name == animationName);
        var isAnimation = animator.runtimeAnimatorController.animationClips.Any(x => x.name.Equals(animationName));
        
        if (isTrigger)
        {
            Log.Info($"Animation: Trigger {animationName} found");
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
                Log.Warn($"Animation: {animationName} never started");
            onFinished(elapsed);
        }
        else if (isAnimation)
        {
            Log.Info($"Animation: {animationName} found");
            var elapsed = 0f; 
            animator.Play(animationName);
            onFinished(elapsed);
        }
        else
        {
            Log.Warn($"Animation: {animationName} not found");
            onFinished(0);
        }
    }
}
