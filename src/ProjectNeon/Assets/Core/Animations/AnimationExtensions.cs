
using System;
using System.Collections;
using UnityEngine;

public static class AnimationExtensions
{
    public static IEnumerator PlayAnimationUntilFinished(this Animator animator, string animationName, Action<float> onFinished, int layer = 0, float maxTime = 5f)
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
}
