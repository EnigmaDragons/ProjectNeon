using UnityEngine;

public class NotInEditor : MonoBehaviour
{
    [SerializeField] private bool enabled = true;
    [SerializeField] private GameObject target;

#if !UNITY_EDITOR
    private void OnEnable() => target.SetActive(enabled);
#endif

#if UNITY_EDITOR
    private void OnEnable() => target.SetActive(false);
#endif
}
