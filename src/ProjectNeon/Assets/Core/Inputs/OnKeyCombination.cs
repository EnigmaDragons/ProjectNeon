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
        var combinationIsPressed = _values.Length != 0;
        foreach(var k in keys)
            if (!Input.GetKey(k))
                combinationIsPressed = false;
        
        if (!_triggered && combinationIsPressed)
            action.Invoke();
        
        _triggered = combinationIsPressed;
    }
}
