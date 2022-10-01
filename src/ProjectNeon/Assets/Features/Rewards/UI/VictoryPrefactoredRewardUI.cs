using System;
using TMPro;
using UnityEngine;

public class VictoryPrefactoredRewardUI : OnMessage<ShowPrefactoredReward, ProceedRequested>
{
    [SerializeField] private GameObject view;
    [SerializeField] private TextMeshProUGUI creditsLabel;
    [SerializeField] private TextMeshProUGUI clinicVouchersLabel;
    [SerializeField] private GameObject creditsPanel;

    private bool _triggeredFinish;
    private Action _onFinished;
    
    protected override void Execute(ShowPrefactoredReward msg)
    {
        _onFinished = msg.OnProceed;
        view.SetActive(true);
        _triggeredFinish = false;
        creditsLabel.text = msg.NumCredits.ToString();
        clinicVouchersLabel.text = msg.NumClinicVouchers.ToString();
        if (creditsPanel != null)
            creditsPanel.SetActive(msg.NumCredits > 0);
    }

    protected override void Execute(ProceedRequested msg)
    {
        if (_triggeredFinish || !view.activeSelf || !msg.ContextName.Equals("PrefactoredVictoryScreen")) return;

        _triggeredFinish = true;
        _onFinished.Invoke();
    }
}
