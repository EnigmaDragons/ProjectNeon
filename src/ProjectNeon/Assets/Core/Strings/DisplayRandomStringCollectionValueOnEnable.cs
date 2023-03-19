using TMPro;
using UnityEngine;

[IgnoreForLocalization]
public class DisplayRandomStringCollectionValueOnEnable : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textArea;
    [SerializeField] private StringKeyValueCollection strings;

    private void OnEnable() => DisplayNext();
    
    public void DisplayNext() => textArea.text = strings.All.Random().Value;
}
