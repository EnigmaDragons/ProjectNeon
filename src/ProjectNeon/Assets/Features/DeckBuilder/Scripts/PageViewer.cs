using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PageViewer : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private GameObject pageTemplate;
    [SerializeField] private Vector2 elementSpace;
    [SerializeField] private GameObject previousPageButton;
    [SerializeField] private TextMeshProUGUI pageNumText;
    [SerializeField] private GameObject nextPageButton;

    private List<Vector2> _elementPositions;

    private List<GameObject> _pages;
    private int _pageIndex;

    public void Init(GameObject elementTemplate, GameObject defaultElementTemplate, 
        List<Action<GameObject>> initElement, Action<GameObject> initDefaultElement, bool keepPageIndex, bool shouldHaveAtLeastOneDefault = false)
    {
        InitIfNeeded();
        
        _pages?.ForEach(Destroy);
        _pages = new List<GameObject>();
        
        for (var i = 0; i < (initElement.Count + (shouldHaveAtLeastOneDefault || initElement.Count == 0 ? 1 : 0)); i += _elementPositions.Count)
            AddPage(elementTemplate, defaultElementTemplate, initElement.Skip(i).Take(_elementPositions.Count).ToList(), initDefaultElement);
        _pageIndex = keepPageIndex ? _pageIndex : 0;
        _pages[_pageIndex].SetActive(true);
        UpdatePageControls();
    }

    public void PreviousPage()
    {
        _pages[_pageIndex].SetActive(false);
        _pageIndex--;
        _pages[_pageIndex].SetActive(true);
        UpdatePageControls();
    }

    public void NextPage()
    {
        _pages[_pageIndex].SetActive(false);
        _pageIndex++;
        _pages[_pageIndex].SetActive(true);
        UpdatePageControls();
    }

    private void Awake() => InitIfNeeded();

    private void InitIfNeeded()
    {
        if (_elementPositions == null)
            CalculateElementPositions();
    }

    private void CalculateElementPositions()
    {
        _elementPositions = new List<Vector2>();
        var pageTransform = pageTemplate.GetComponent<RectTransform>();
        var columns = (int)Math.Floor(pageTransform.sizeDelta.x / elementSpace.x);
        var rows = (int)Math.Floor(pageTransform.sizeDelta.y / elementSpace.y);
        for (var row = 0; row < rows; row++)
            for (var column = 0; column < columns; column++)
                _elementPositions.Add(new Vector2((column - (columns / 2f - 0.5f)) * elementSpace.x, (row - (rows / 2f - 0.5f)) * -elementSpace.y));
    }

    private void AddPage(GameObject elementTemplate, GameObject defaultElementTemplate, List<Action<GameObject>> initElement, Action<GameObject> initDefaultElement)
    {
        var page = Instantiate(pageTemplate, parent);
        for (var i = 0; i < _elementPositions.Count; i++)
        {
            var element = Instantiate(i < initElement.Count ? elementTemplate : defaultElementTemplate, page.transform);
            element.GetComponent<RectTransform>().anchoredPosition = _elementPositions[i];
            if (i < initElement.Count)
                initElement[i](element);
            else
                initDefaultElement(element);
        }
        page.SetActive(false);
        _pages.Add(page);
    }

    private void UpdatePageControls()
    {
        previousPageButton.SetActive(_pageIndex != 0);
        nextPageButton.SetActive(_pageIndex != _pages.Count - 1);
        pageNumText.gameObject.SetActive(_pages.Count != 1);
        pageNumText.text = $"{(_pageIndex + 1)} / {_pages.Count}";
    }
}
