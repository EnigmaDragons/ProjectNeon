using I2.Loc;
using UnityEngine;
using UnityEngine.EventSystems;

public class GlobalEffectPresenter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Localize shortDescLabel;

    private GlobalEffect _e;

    public void Init(GlobalEffect e)
    {
        _e = e;
        shortDescLabel.SetTerm(e.ShortDescriptionTerm);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Message.Publish(new ShowTooltip(transform, _e.FullDescriptionTerm.ToLocalized()));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Message.Publish(new HideTooltip());
    }
}
