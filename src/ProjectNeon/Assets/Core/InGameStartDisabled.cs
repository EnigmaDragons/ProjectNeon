using UnityEngine;

public sealed class InGameStartDisabled : MonoBehaviour
{
    [SerializeField] private GameObject target;

    private void Awake() => target.SetActive(false);
}
