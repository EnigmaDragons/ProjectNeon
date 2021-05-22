using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectTabButton : MonoBehaviour
{
    [SerializeField] private Image selected;
    [SerializeField] private Button button;

    public void Init(Action onClick, bool isSelected)
    {
        button.onClick.AddListener(() => onClick());
        SetSelected(isSelected);
    }

    public void SetSelected(bool isSelected)
    {
        selected.gameObject.SetActive(isSelected);
    }
}