
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverDebugger : MonoBehaviour
{
    [SerializeField] private KeyCode key;

    private void Update()
    {
        if (!Input.GetKeyDown(key)) 
            return;
        
        var pointer = new PointerEventData(EventSystem.current) {position = Input.mousePosition};
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        foreach (var g in raycastResults)
            Debug.Log($"Mouse Raycast - {g.gameObject.name}", g.gameObject);
    }
}
