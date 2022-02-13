using UnityEngine;

public class ToggleTargetIfNoModalOpen : MonoBehaviour
{
    [SerializeField] private StaticModalState modalState;
    [SerializeField] private GameObject[] targets = new GameObject[1];
    
    public void Toggle()
    {
        if (modalState.Current.IsMissing)
            targets.ForEach(t => t.SetActive(!t.activeSelf));
    }
}
