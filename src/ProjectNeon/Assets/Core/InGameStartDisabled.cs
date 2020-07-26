using UnityEngine;

public sealed class InGameStartDisabled : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject[] additional;

    private void Awake()
    {
        var targetObj = target != null ? target : gameObject;
        targetObj.SetActive(false);
        additional.ForEach(a => a.SetActive(false));
    }
}
