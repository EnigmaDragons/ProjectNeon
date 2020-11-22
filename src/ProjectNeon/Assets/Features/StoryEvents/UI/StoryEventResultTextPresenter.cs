
using TMPro;
using UnityEngine;

public class StoryEventResultTextPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

    public void Init(string text) => label.text = text;
}
