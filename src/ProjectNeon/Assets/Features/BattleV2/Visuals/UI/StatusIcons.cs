using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/StatusIcons")]
public class StatusIcons : ScriptableObject
{
    [SerializeField] private List<StatusIconDefinition> statusIcons;

    public Dictionary<string, StatusIconDefinition> Icons => statusIcons.ToDictionary(x => x.Name, x => x);
    public StatusIconDefinition this[TemporalStatType stat] => Icons.VerboseGetValue(stat.ToString(), nameof(statusIcons));
    public StatusIconDefinition this[StatType stat] => Icons.VerboseGetValue(stat.ToString(), nameof(statusIcons));
    public StatusIconDefinition this[StatusTag status] => Icons.VerboseGetValue(status.ToString(), nameof(statusIcons));
    public StatusIconDefinition this[string status] => Icons.VerboseGetValue(status, nameof(statusIcons));
}
