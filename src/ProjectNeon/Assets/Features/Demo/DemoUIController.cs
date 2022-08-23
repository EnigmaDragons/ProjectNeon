using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DemoUIController : MonoBehaviour
{
    [SerializeField] private BoolVariable isDemo;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Button startAdventure;
    [SerializeField] private Button startDraft;

    private void Start()
    {
        if (isDemo.Value)
        {
            text.text = $"Runs Remaining In Demo: {Math.Max(3 - CurrentProgressionData.Data.RunsFinished, 0)}";
            if (CurrentProgressionData.Data.RunsFinished >= 3)
            {
                startAdventure.interactable = false;
                startDraft.interactable = false;
            }
        }
    }
}