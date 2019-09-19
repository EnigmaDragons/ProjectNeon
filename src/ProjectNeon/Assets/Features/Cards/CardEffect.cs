using UnityEngine;

[CreateAssetMenu()]
public class CardEffect : ScriptableObject
{
    [SerializeField] private EffectAction Effect;
    [SerializeField] private TargetGroup TargetGroup;
    [SerializeField] private TargetScope TargetScope;
}
