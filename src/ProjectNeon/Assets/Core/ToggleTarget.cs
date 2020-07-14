using UnityEngine;

public sealed class ToggleTarget : MonoBehaviour
{
    [SerializeField] private GameObject[] targets;
    
    public void Toggle()
    {
        targets.ForEach(t => t.SetActive(!t.activeSelf));
    }
}

