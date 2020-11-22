using System;
using TMPro;
using UnityEngine;

[Obsolete("MapView1")]
public class CurrentAdventureProgress : MonoBehaviour
{
    [SerializeField] private AdventureProgress adventure;
    [SerializeField] private TextMeshProUGUI nextText;

    private void Awake()
    {
        adventure.InitIfNeeded();
        nextText.text = adventure.CurrentStageSegment.Name;
    }
}
