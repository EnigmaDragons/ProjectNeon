
using System;
using System.Collections;
using UnityEngine;

public static class GameObjectExtensions
{
    public static void ExecuteAfterDelay(this MonoBehaviour o, Action a, float secondsDelay) 
        => o.StartCoroutine(ExecuteAfterDelay(a, secondsDelay));

    private static IEnumerator ExecuteAfterDelay(Action a, float secondsDelay)
    {
        yield return new WaitForSeconds(secondsDelay);
        a();
    }
}
