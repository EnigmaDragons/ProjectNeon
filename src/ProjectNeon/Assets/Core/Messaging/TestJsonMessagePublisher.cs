using System;
using UnityEngine;

public sealed class TestJsonMessagePublisher : MonoBehaviour
{
    [SerializeField] private string messageType;
    [SerializeField] private string json;

    public void Publish()
    {
        var type = Type.GetType(messageType);
        var msg = JsonUtility.FromJson(json, type);
        Message.Publish(msg);
        Debug.Log($"Published Test Message of Type '{type}'", gameObject);
    }
}
