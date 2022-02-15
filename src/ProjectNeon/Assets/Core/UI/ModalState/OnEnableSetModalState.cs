using UnityEngine;

public class OnEnableSetModalState : MonoBehaviour
{
    [SerializeField] private StaticModalState state;
    [SerializeField] private string modalStateName;
    [SerializeField] private bool clearOnAwake = true;
    [SerializeField] private bool shouldClearOnDisable = true;

    private void Awake()
    {
        if (clearOnAwake)
            state.Clear();
    }

    private void OnEnable() => state.Set(modalStateName);
    
    private void OnDisable()
    {
        if (shouldClearOnDisable)
            state.Clear(modalStateName);
    }
}
