using System;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DemoUIController : MonoBehaviour, ILocalizeTerms
{
    [SerializeField] private BoolVariable isDemo;
    [SerializeField] private Localize text;
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
            text.SetFinalText($"{"Menu/Demo Runs Remaining".ToLocalized()}: {Math.Max(maxRuns - runsFinished, 0)}");
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

    public string[] GetLocalizeTerms()
        => new[] {"Menu/Demo Runs Remaining"};
}