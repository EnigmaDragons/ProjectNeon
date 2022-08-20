using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI changes;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Button selectButton;

    public void Init(Difficulty difficulty, Action onSelect)
    {
        nameText.text = difficulty.Name;
        changes.text = difficulty.Changes;
        description.text = difficulty.Description;
        
        selectButton.onClick.AddListener(() => onSelect());
    }
}