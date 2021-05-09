using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArchetypeToggle : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private Image image;
    [SerializeField] private ArchetypeIcons icons;

    public void Init(string archetype, Action<bool> action)
    {
        image.sprite = icons.ForArchetypes(new HashSet<string> {archetype});
        toggle.onValueChanged.AddListener(x => action(x));
    }
}