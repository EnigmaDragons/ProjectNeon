using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentAdventureProgress : MonoBehaviour
{
    [SerializeField] private AdventureProgress adventure;
    [SerializeField] private TextMeshProUGUI nextText;

    private void OnEnable()
    {
        if (!adventure.HasStageBegun)
        {
            adventure.Advance();
        }
        nextText.text = adventure.CurrentStageSegment.Name;
    }
}
