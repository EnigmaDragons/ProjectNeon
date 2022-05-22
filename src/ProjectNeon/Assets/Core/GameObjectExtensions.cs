using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

public static class GameObjectExtensions
{    
    public static void ExecuteAfterTinyDelay(this MonoBehaviour o, Action a) 
        => ExecuteAfterDelay(o, a, 0.02f);
    
    public static void ExecuteAfterDelay(this MonoBehaviour o, float secondsDelay, Action a)
        => ExecuteAfterDelay(o, a, secondsDelay);
    public static void ExecuteAfterDelay(this MonoBehaviour o, Action a, float secondsDelay)
    {
        if (o.gameObject.activeSelf)
            o.StartCoroutine(ExecuteAfterDelayIfGameObjectActive(o, a, secondsDelay));
    }
    
    private static IEnumerator ExecuteAfterDelay(Action a, float secondsDelay)
    {
        yield return new WaitForSeconds(secondsDelay);
        a();
    }
    
    private static IEnumerator ExecuteAfterDelayIfGameObjectActive(MonoBehaviour o, Action a, float secondsDelay)
    {
        yield return new WaitForSeconds(secondsDelay);
        if (o.gameObject.activeSelf)
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
    
    public static void DestroyAllChildrenImmediate(this Transform t)
    {
        foreach(Transform child in t)
            Object.DestroyImmediate(child.gameObject);
    }
    
    public static void DestroyAllChildrenImmediate(this GameObject o)
    {
        foreach(Transform child in o.transform)
            Object.DestroyImmediate(child.gameObject);
    }
}
