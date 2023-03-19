
using TMPro;
using UnityEngine;

[IgnoreForLocalization]
public class StoryEventResultTextPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

    public void Init(string text) => label.text = text;
}
