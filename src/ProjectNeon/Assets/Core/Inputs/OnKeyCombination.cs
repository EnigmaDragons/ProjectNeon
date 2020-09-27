using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class OnKeyCombination : MonoBehaviour
{
    [SerializeField] private KeyCode[] keys;
    [SerializeField] private UnityEvent action;

    private bool _triggered;
    private int[] _values;

    private void Awake()
    {
        _values = keys.Select(k => (int)k).ToArray();
    }
    
    private void Update()
    {
        var combinationIsPressed = _values.All(v => Input.GetKey((KeyCode)v));
        if (!_triggered && combinationIsPressed)
            action.Invoke();
        _triggered = combinationIsPressed;
    }
}
