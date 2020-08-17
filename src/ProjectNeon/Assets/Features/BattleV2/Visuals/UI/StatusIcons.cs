using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/StatusIcons")]
public class StatusIcons : ScriptableObject
{
    [SerializeField] private List<StatusIconDefinition> statusIcons;

    public Dictionary<string, StatusIconDefinition> Icons => statusIcons.ToDictionary(x => x.Name, x => x);
    public StatusIconDefinition this[TemporalStatType stat] => Icons[stat.ToString()];
    public StatusIconDefinition this[StatType stat] => Icons[stat.ToString()];
    public StatusIconDefinition this[StatusTag status] => Icons[status.ToString()];


}
