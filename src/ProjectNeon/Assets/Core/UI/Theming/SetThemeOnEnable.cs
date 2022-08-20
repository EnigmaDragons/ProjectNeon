using UnityEngine;

public class SetThemeOnEnable : MonoBehaviour
{
    [SerializeField] private CurrentTheme currentTheme;
    [SerializeField] private CustomTheme customTheme;

    private void OnEnable() => currentTheme.Set(customTheme);
}
