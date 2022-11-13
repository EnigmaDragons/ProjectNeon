using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LanguageButtonPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image image;
    
    private Action _onSelect;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnSelect);
    }
    
    public void Init(LanguageOption o, Action onSelect)
    {
        _onSelect = onSelect;
        text.text = o.OptionText;
        text.font = o.OptionFont;
        image.sprite = o.FlagSprite;
    }

    private void OnSelect() => _onSelect();
}
