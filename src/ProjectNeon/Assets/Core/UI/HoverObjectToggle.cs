using UnityEngine;
using UnityEngine.EventSystems;

public class HoverObjectToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject target;

    private void Awake() => target.SetActive(false);
    
    public void OnPointerEnter(PointerEventData eventData) => Show();
    public void OnPointerExit(PointerEventData eventData) => Hide();
    
    public void Show()
    {
        if (gameObject.activeSelf && target != null)
            target.SetActive(true);
    }

    public void Hide()
    {
        if (gameObject.activeSelf && target != null)
            target.SetActive(false);
    }
}
