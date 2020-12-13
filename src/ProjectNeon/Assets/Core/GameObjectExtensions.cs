using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

public static class GameObjectExtensions
{
    public static void ExecuteAfterDelay(this MonoBehaviour o, Action a, float secondsDelay) 
        => o.StartCoroutine(ExecuteAfterDelay(a, secondsDelay));
    
    private static IEnumerator ExecuteAfterDelay(Action a, float secondsDelay)
    {
        yield return new WaitForSeconds(secondsDelay);
        a();
    }

    public static void DestroyAllChildren(this GameObject o)
    {
        foreach(Transform child in o.transform)
            Object.Destroy(child.gameObject);
    }
    
    public static void DestroyAllChildren(this Transform t)
    {
        foreach(Transform child in t)
            Object.Destroy(child.gameObject);
    }
}
