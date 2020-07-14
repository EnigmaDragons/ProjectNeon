using UnityEngine;

public sealed class OnlyEnabledForDesktop : MonoBehaviour
{
    [SerializeField] private GameObject target;
    
    private void Awake()
    {
        var t = target == null ? gameObject : target;
        if (Application.isMobilePlatform || Application.platform == RuntimePlatform.WebGLPlayer)
            t.SetActive(false);
    }
}
