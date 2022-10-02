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
    [SerializeField] private GameObject zeroRunsRemainingView;

    private void Start()
    {
        if (isDemo.Value)
        {
            var runsFinished = CurrentProgressionData.Data.RunsFinished;
            var maxRuns = 3;
            var outOfRuns = runsFinished >= maxRuns; 
            text.gameObject.SetActive(true);
            text.text = $"Demo Runs Remaining: {Math.Max(maxRuns - runsFinished, 0)}";
            zeroRunsRemainingView.SetActive(outOfRuns);
            if (outOfRuns)
            {
                startAdventure.interactable = false;
                startDraft.interactable = false;
            }
        }
        else 
            text.gameObject.SetActive(false);
    }
}