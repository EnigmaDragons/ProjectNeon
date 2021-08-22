using TMPro;
using UnityEngine;

public class BindLongText : MonoBehaviour
{
    [SerializeField] private LongText value;
    [SerializeField] private TextMeshProUGUI textArea;

    void Start() => textArea.text = value;
}
