using UnityEngine;

public class PublishInteractedWithOnEnable : MonoBehaviour
{
    [SerializeField] private string uiElement;
    
    private void OnEnable()
    {
        AllMetrics.PublishInteractedWith(uiElement);
    }
}
