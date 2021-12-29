using UnityEngine;

public class OnlyInEditor : MonoBehaviour
{
    [SerializeField] private bool enabled = true;

#if !UNITY_EDITOR
    private void OnEnable() => gameObject.SetActive(false);
#endif

#if UNITY_EDITOR
    private void OnEnable() => gameObject.SetActive(enabled);
#endif
}
