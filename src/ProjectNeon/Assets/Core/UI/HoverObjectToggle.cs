using UnityEngine;
using UnityEngine.EventSystems;

public class HoverObjectToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject target;

    private void Awake() => target.SetActive(false);
    
    public void OnPointerEnter(PointerEventData eventData) => Show();
    public void OnPointerExit(PointerEventData eventData) => Hide();
    
    public void Show() => target.SetActive(true);
    public void Hide() => target.SetActive(false);
}
