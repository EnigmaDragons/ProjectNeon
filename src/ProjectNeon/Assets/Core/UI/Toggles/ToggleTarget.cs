using UnityEngine;

public sealed class ToggleTarget : MonoBehaviour
{
    [SerializeField] private GameObject[] targets = new GameObject[1];
    
    public void Toggle()
    {
        targets.ForEach(t => t.SetActive(!t.activeSelf));
    }
}
