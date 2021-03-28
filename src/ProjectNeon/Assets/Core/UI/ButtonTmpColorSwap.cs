using TMPro;
using UnityEngine;

public class ButtonTmpColorSwap : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Color hoverColor;

    private Color _defaultColor;
    
    private void Awake() => _defaultColor = text.color;

    public void OnPointerEnter() => text.color = hoverColor;

    public void OnPointerExit() => text.color = _defaultColor;
}
