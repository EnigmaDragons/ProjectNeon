using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArchetypeToggle : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private TintToggle tintToggle;
    [SerializeField] private ArchetypeTints tints;

    public void Init(string archetype, Action<bool> action)
    {
        var color = tints.ForArchetypes(new HashSet<string> {archetype});
        tintToggle.on = new Color(color.r, color.g, color.b, tintToggle.on.a);;
        tintToggle.off = new Color(color.r, color.g, color.b, tintToggle.off.a);
        tintToggle.UpdateTint(false);
        toggle.onValueChanged.AddListener(x => action(x));
    }
}