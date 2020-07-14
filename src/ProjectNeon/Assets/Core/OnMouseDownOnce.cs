using UnityEngine;
using UnityEngine.Events;

public sealed class OnMouseDownOnce : MonoBehaviour
{
    [SerializeField] private UnityEvent action;

    private bool _triggered = false;
    
    private void Update()
    {
        if (_triggered || !Input.GetMouseButtonDown(0)) return;
        
        _triggered = true; 
        action.Invoke();
    }
}
