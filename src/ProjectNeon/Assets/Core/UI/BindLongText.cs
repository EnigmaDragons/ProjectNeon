using TMPro;
using UnityEngine;

public class BindLongText : MonoBehaviour
{
    [SerializeField] private LongText value;
    [SerializeField] private TextMeshProUGUI textArea;
    [SerializeField] private float heightSizeFactor = 1;

    void Start()
    {
        textArea.text = value;
        
        var rectTransform = textArea.gameObject.transform.GetComponent<RectTransform>();
        var preferredValues = textArea.GetPreferredValues(value);
        var preferredHeight = preferredValues.y * heightSizeFactor;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, preferredHeight);
    }
}
