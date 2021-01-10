using System;
using UnityEngine;

public class BattleVFX : MonoBehaviour
{
    [SerializeField] private StringVariable effectName;
    [SerializeField] private float durationSeconds;
    [SerializeField] public float maxLifetime;
    [SerializeField] private bool waitForCompletion = true;

    private float _liveDurationSeconds;
    private float _liveMaxLifetime;
    
    public string EffectName => effectName == null ? "" : effectName.Value;
    public float DurationSeconds => _liveDurationSeconds;
    public bool WaitForCompletion => waitForCompletion;

    private float _lifetime;

    public void SetSpeed(float speed)
    {
        _liveDurationSeconds = durationSeconds * speed;
        _liveMaxLifetime = maxLifetime * speed;
    }
    
    private void Update()
    {
        _lifetime += Time.deltaTime;
        if (_lifetime > Math.Max(_liveDurationSeconds, _liveMaxLifetime))
            Destroy(gameObject);
    }
}
