using UnityEngine;

public class OnlyInEditor : MonoBehaviour
{
#if !UNITY_EDITOR
    private void OnEnable() => gameObject.SetActive(false);
#endif
}
