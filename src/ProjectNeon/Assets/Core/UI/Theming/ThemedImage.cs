using UnityEngine;
using UnityEngine.UI;

public class ThemedImage : OnMessage<CurrentThemeChanged>
{
    [SerializeField] private CurrentTheme currentTheme;
    [SerializeField] private string elementCategoryName;
    [SerializeField] private Image[] imgs;
    
    private void Start() => Render();
    protected override void Execute(CurrentThemeChanged msg) => Render();

    private void Render()
    {
        var theme = currentTheme.Value;
        if (theme == null)
        {
            Log.Error("Current Theme is Null");
            return;
        }

        var c = theme.GetColor(elementCategoryName);
        imgs.ForEach(t => t.color = c);
    }
}
