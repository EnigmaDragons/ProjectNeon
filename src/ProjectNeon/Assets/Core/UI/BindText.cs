using TMPro;
using UnityEngine;

[IgnoreForLocalization]
public class BindText : MonoBehaviour
{
    [SerializeField] private StringReference value;
    [SerializeField] private TextMeshProUGUI textArea;

    void Start() => textArea.text = value;
}
