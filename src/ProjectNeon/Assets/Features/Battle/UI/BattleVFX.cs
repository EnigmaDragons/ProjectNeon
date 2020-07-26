using UnityEngine;

public class BattleVFX : MonoBehaviour
{
    [SerializeField] private StringVariable effectName;
    [SerializeField] private float durationSeconds;
    [SerializeField] private bool waitForCompletion = true;

    public string EffectName => effectName.Value;
    public float DurationSeconds => durationSeconds;
    public bool WaitForCompletion => waitForCompletion;
}
