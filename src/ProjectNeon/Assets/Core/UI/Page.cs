using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class Page : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button actionButton;
    
    public void Init(Action prev, Action next, Action action)
    {
        if (previousButton != null)
            previousButton.onClick.AddListener(() => prev());
        if (nextButton != null)
            nextButton.onClick.AddListener(() => next());
        if (actionButton != null)
            actionButton.onClick.AddListener(() => action());
    }
}
