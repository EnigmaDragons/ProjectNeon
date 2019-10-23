using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceCounterPresenter : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI counter;

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    public void Init(IResourceType resource)
    {
        icon.sprite = resource.Icon;
        counter.text = $"{resource.StartingAmount}/{resource.MaxAmount}";
    }
}
