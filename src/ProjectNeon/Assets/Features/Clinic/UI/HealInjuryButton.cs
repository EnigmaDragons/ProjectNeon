using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealInjuryButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI injuryLabel;
    [SerializeField] private TextMeshProUGUI costLabel;
    [SerializeField] private Button _button;

    public void Init(string injury, int cost, Action action)
    {
        injuryLabel.text = injury;
        costLabel.text = cost.ToString();
        _button.onClick.AddListener(() => action());
    }
}