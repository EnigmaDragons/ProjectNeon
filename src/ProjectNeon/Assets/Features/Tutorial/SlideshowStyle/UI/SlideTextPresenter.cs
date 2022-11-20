using TMPro;
using UnityEngine;

[IgnoreForLocalization]
public class SlideTextPresenter : SlideTextPresenterBase
{
    [SerializeField] private TextMeshProUGUI slideText;
    
    protected override void InitText(string text) => slideText.text = text;
}