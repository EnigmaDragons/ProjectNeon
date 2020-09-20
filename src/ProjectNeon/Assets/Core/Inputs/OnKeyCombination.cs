using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class OnKeyCombination : MonoBehaviour
{
    [SerializeField] private KeyCode[] keys;
    [SerializeField] private UnityEvent action;

    private bool _triggered;
    
    private void Update()
    {
        var combinationIsPressed = keys.All(Input.GetKey);
        if (!_triggered && combinationIsPressed)
            action.Invoke();
        _triggered = combinationIsPressed;
    }
}
