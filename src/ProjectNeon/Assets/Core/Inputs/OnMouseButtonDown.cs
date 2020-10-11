using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class OnMouseButtonDown : MonoBehaviour
{
    [SerializeField] private MouseButton[] buttons = { MouseButton.LeftMouse };
    [SerializeField] private UnityEvent action;

    private void Update()
    {
        if (buttons.Any(b => Input.GetMouseButtonDown((int)b)))
            action.Invoke();
    }
}
