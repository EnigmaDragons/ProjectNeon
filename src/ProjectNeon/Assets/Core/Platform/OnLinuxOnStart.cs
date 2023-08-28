using UnityEngine;
using UnityEngine.Events;

public class OnLinuxOnStart : MonoBehaviour
{
    [SerializeField] private UnityEvent onLinuxOnStart;
    
#if LINUX
    private void Start()
    {
        onLinuxOnStart.Invoke();
    }
#endif
}
