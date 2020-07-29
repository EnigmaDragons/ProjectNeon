using System;
using UnityEngine;

public class BattleVFX : MonoBehaviour
{
    [SerializeField] private StringVariable effectName;
    [SerializeField] private float durationSeconds;
    [SerializeField] private float maxLifetime;
    [SerializeField] private bool waitForCompletion = true;

    public string EffectName => effectName.Value;
    public float DurationSeconds => durationSeconds;
    public bool WaitForCompletion => waitForCompletion;

    private float _lifetime;

    private void Update()
    {
        _lifetime += Time.deltaTime;
        if (_lifetime > Math.Max(durationSeconds, maxLifetime))
            Destroy(gameObject);
    }
}
