using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Theme/CustomTheme")]
public class CustomTheme : ScriptableObject
{
    [SerializeField] private Color primaryColor;
    [SerializeField] private NamedColor[] colors;

    private Dictionary<string, Color> _colors;

    public Color GetColor(string elementCategoryName)
    {
        InitializeIfNeeded();
        return _colors.TryGetValue(elementCategoryName, out Color color) ? color : primaryColor;
    }

    private void InitializeIfNeeded()
    {
        if (_colors == null)
            _colors = colors.SafeToDictionary(c => c.Name, c => c.Color);
    }
}
