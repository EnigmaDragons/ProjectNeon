using UnityEngine;
using UnityEngine.EventSystems;

public class HoverElementDebugger : MonoBehaviour
{
    private GameObject _currentElement;
    
    private void Update() 
    {
        if (EventSystem.current.IsPointerOverGameObject() && EventSystem.current.currentSelectedGameObject != null)
            Log.Info("Mouse Over: " + EventSystem.current.currentSelectedGameObject.name);
    
        var newElement = EventSystem.current.currentSelectedGameObject;
        if (_currentElement != null && _currentElement != newElement)
            Log.Info($"Changed Hover Element {_currentElement.name}", _currentElement);
        _currentElement = newElement;
    }
}
