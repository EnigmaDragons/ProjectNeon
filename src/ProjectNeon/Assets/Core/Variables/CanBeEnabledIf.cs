using UnityEngine;

public sealed class CanBeEnabledIf : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private bool enabled = true;
    [SerializeField] private BoolVariable variable;

    private void OnEnable()
    {
        target.SetActive(variable.Value == enabled);
    }
}
