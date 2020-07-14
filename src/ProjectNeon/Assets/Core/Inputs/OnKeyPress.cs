using UnityEngine;
using UnityEngine.Events;

public sealed class OnKeyPress : MonoBehaviour
{
    [SerializeField] private KeyCode[] keys;
    [SerializeField] private UnityEvent action;

    private void Update()
    {
        foreach (var k in keys)
        {
            if (!Input.GetKeyDown(k)) 
                continue;
            
            action.Invoke();
            return;
        }
    }
}
