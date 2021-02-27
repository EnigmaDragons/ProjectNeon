using UnityEngine;
using System.Collections.Generic;

public sealed class LightFlickerEffect : MonoBehaviour 
{
    [Tooltip("External light to flicker; you can leave this null if you attach script to a light")]public new Light light;
    [Tooltip("Minimum random light intensity")] public float minIntensity = 0f;
    [Tooltip("Maximum random light intensity")] public float maxIntensity = 1f;
    [Tooltip("How much to smooth out the randomness; lower values = sparks, higher = lantern")] [Range(1, 50)] public int smoothing = 5;

    private Queue<float> _smoothQueue;
    private float _lastSum = 0;

    private void Awake()
    {
        _smoothQueue = new Queue<float>(smoothing);
        if (light == null)
            light = GetComponent<Light>();
        if (light == null)
            Destroy(this);
    }

    public void Reset() 
    {
        _smoothQueue.Clear();
        _lastSum = 0;
    }

    public void Update() 
    {
        if (light == null || _smoothQueue == null)
            return;

        // pop off an item if too big
        while (_smoothQueue.Count >= smoothing) {
            _lastSum -= _smoothQueue.Dequeue();
        }

        // Generate random new item, calculate new average
        float newVal = Random.Range(minIntensity, maxIntensity);
        _smoothQueue.Enqueue(newVal);
        _lastSum += newVal;

        // Calculate new smoothed average
        light.intensity = _lastSum / (float)_smoothQueue.Count;
    }
}
