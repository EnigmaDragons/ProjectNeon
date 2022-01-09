using System;
using System.Collections.Generic;
using UnityEngine;

public class MemberSpecialPowersUi : MonoBehaviour
{
    [SerializeField] private StatusIcon[] icons;
    
    public void UpdateStatuses(List<CurrentStatusValue> statuses)
    {
        for (var i = 0; i < Math.Max(statuses.Count, icons.Length); i++)
        {
            if (i < Math.Min(statuses.Count, icons.Length))
                icons[i].Show(statuses[i]);
            else if (i < icons.Length)
                icons[i].gameObject.SetActive(false);
        }
    }
}
