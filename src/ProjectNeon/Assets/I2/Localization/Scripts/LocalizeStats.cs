using System;
using System.Collections.Generic;
using UnityEngine;

namespace I2.Loc
{
    [CreateAssetMenu(menuName = "OnlyOnce/Localize Stats")]
    public class LocalizeStats : ScriptableObject, ILocalizeTerms
    {
        public string[] GetLocalizeTerms()
        {
            var results = new List<string>();
            foreach (StatType stat in Enum.GetValues(typeof(StatType)))
                results.Add($"Stats/Stat-{stat.ToString()}");
            foreach (TemporalStatType stat in Enum.GetValues(typeof(TemporalStatType)))
                results.Add($"Stats/Stat-{stat.ToString()}");
            return results.ToArray();
        }
    }
}