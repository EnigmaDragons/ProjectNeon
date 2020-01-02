using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public sealed class OnKeyPress : MonoBehaviour
{
    [SerializeField] private KeyCode[] keys;
    [SerializeField] private UnityEvent action;

    private void Update()
    {
        if (keys.Any(Input.GetKeyDown))
            action.Invoke();
    }
}
