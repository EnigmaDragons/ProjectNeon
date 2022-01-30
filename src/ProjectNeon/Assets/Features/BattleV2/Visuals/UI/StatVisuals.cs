using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/StatIcons")]
public class StatVisuals : ScriptableObject
{
    [SerializeField] private StatIconAndColor[] stats;

    public Maybe<StatIconAndColor> Get(string statName) 
        => stats.FirstOrMaybe(s => s.StatType.Value.Equals(statName));
}
