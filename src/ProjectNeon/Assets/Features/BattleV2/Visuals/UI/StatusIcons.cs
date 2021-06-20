using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/StatusIcons")]
public class StatusIcons : ScriptableObject
{
    [SerializeField] private Sprite missingStatusIconSprite;
    [SerializeField] private List<StatusIconDefinition> statusIcons;

    private Dictionary<string, StatusIconDefinition> Icons => statusIcons.ToDictionary(x => x.Name, x => x);
    public StatusIconDefinition this[TemporalStatType stat] => this[stat.ToString()];
    public StatusIconDefinition this[StatType stat] => this[stat.ToString()];
    public StatusIconDefinition this[StatusTag status] => this[status.ToString()];
    public StatusIconDefinition this[string status] => Icons.ValueOrDefault(status,
        () => new StatusIconDefinition {Name = $"Missing Status Icon for {status}", Icon = missingStatusIconSprite});
}
