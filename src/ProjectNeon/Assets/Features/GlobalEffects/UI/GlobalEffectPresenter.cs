using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GlobalEffectPresenter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI shortDescLabel;

    private GlobalEffect _e;

    public void Init(GlobalEffect e)
    {
        _e = e;
        shortDescLabel.text = e.ShortDescription;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Message.Publish(new ShowTooltip(transform, _e.FullDescription));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Message.Publish(new HideTooltip());
    }
}
