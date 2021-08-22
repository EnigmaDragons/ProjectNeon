using TMPro;
using UnityEngine;

public class BindText : MonoBehaviour
{
    [SerializeField] private StringReference value;
    [SerializeField] private TextMeshProUGUI textArea;

    void Start() => textArea.text = value;
}
