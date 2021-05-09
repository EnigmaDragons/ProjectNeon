using TMPro;
using UnityEngine;

public class ButtonTmpColorSwap : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Color hoverColor;

    private Color _defaultColor;
    
    private void Awake() => _defaultColor = text.color;

    private void OnEnable() => Reset();
    private void OnDisable() => Reset();

    public void OnPointerEnter() => text.color = hoverColor;
    public void OnPointerExit() => Reset();
    
    private void Reset() => text.color = _defaultColor;
}
