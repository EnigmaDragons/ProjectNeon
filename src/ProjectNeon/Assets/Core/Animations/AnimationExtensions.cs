
using System;
using System.Collections;
using UnityEngine;

public static class AnimationExtensions
{
    public static IEnumerator PlayAnimationUntilFinished(this Animator animator, string animationName, Action<float> onFinished, int layer = 0, float maxTime = 5f)
    {
        var elapsed = 0f; 
        animator.Play(animationName, layer);
    
        yield return new WaitForSeconds(0.1f);
        elapsed += 0.1f;
    
        bool AnimationIsActive() => animator.GetCurrentAnimatorStateInfo(layer).IsName(animationName);
        bool AnimationIsStillPlaying() => animator.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1.0f;
        while (AnimationIsActive() && AnimationIsStillPlaying())
        {
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
            if (elapsed > maxTime)
                break;
        }

        onFinished(elapsed);
    }
}
