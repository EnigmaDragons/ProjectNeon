using TMPro;
using UnityEngine;

public class LabelPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

    public void Init(string text) => label.text = text;

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
