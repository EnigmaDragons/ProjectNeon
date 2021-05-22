using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TintToggle : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private Image[] images;
    [SerializeField] private TextMeshProUGUI[] texts;
    [SerializeField] public Color on;
    [SerializeField] public Color off;

    private void Awake()
    {
        toggle.onValueChanged.AddListener(x => UpdateTint(x));
        UpdateTint(toggle.isOn);
    }

    private void OnEnable() => UpdateTint(toggle.isOn);

    public void UpdateTint(bool isOn)
    {
        images.ForEach(image => image.color = isOn ? on : off);
        texts.ForEach(text => text.color = isOn ? on : off);
    }
}