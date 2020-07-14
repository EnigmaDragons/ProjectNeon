using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class OnKeyCombination : MonoBehaviour
{
    [SerializeField] private KeyCode[] keys;
    [SerializeField] private UnityEvent action;

    private void Update()
    {
        if (keys.All(Input.GetKey))
            action.Invoke();
    }
}
