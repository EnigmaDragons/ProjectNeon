using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NonDestructivePageViewer : MonoBehaviour
{
    [SerializeField] private GameObject[] elements;
    [SerializeField] private GameObject previousPageButton;
    [SerializeField] private GameObject nextPageButton;

    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI pageNumText;

    [SerializeField] private bool bindButtons;

    private int _pageIndex;
    private bool _isInitialized;
    private List<Action<GameObject>> _initElements = new List<Action<GameObject>>();
    private Action<GameObject> _initAsDefaultElement = _ => { };
    private int PageCount => Math.Max(1, (int)Math.Ceiling((decimal)_initElements.Count / elements.Length));

    public void Init(List<Action<GameObject>> initElement, Action<GameObject> initAsDefaultElement, bool keepPageIndex)
    {
        InitIfNeeded();
        _initElements = initElement;
        _initAsDefaultElement = initAsDefaultElement;
        
        if (PageCount > 0)
        {
            _pageIndex = keepPageIndex && PageCount > _pageIndex 
                ? _pageIndex 
                : 0;
            InitPage();
        }
        
        UpdatePageControls();
    }

    public void PreviousPage()
    {
        _pageIndex--;
        InitPage();
        UpdatePageControls();
    }

    public void NextPage()
    {
        _pageIndex++;
        InitPage();
        UpdatePageControls();
    }

    private void Awake() => InitIfNeeded();

    private void InitIfNeeded()
    {
        if (_isInitialized)
            return;
        
        _isInitialized = true;
        if (bindButtons)
        {
            nextPageButton.GetComponent<Button>().onClick.AddListener(NextPage);
            previousPageButton.GetComponent<Button>().onClick.AddListener(PreviousPage);
        }
    }

    private void InitPage()
    {
        var currentElements = _initElements.Skip(_pageIndex * elements.Length).Take(elements.Length).ToArray();
        for (var i = 0; i < elements.Length; i++)
        {
            if (currentElements.Length > i)
                currentElements[i](elements[i]);
            else
                _initAsDefaultElement(elements[i]);
        }
    }

    private void UpdatePageControls()
    {
        if (previousPageButton != null)
            previousPageButton.SetActive(PageCount > 0 && _pageIndex != 0);
        if (nextPageButton != null)
            nextPageButton.SetActive(_pageIndex != PageCount - 1);
        if (pageNumText != null)
        {
            pageNumText.gameObject.SetActive(PageCount > 1);
            pageNumText.text = $"{(_pageIndex + 1)} / {PageCount}";
        }
    }
}