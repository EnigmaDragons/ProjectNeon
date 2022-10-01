using UnityEngine;

public class InitTheme : MonoBehaviour
{
    [SerializeField] private CurrentTheme currentTheme;
    [SerializeField] private CustomTheme customTheme;

    private void Awake() => currentTheme.SetIfNonePresent(customTheme);
}
