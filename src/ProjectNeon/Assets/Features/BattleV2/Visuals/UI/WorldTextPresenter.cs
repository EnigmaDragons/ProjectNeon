using TMPro;
using UnityEngine;

[IgnoreForLocalization]
public class WorldTextPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshPro label;
    
    public void Init(string text)
    {
        label.text = text;
    }
}
