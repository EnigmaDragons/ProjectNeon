using TMPro;
using UnityEngine;

public class ThemedTextMeshProGUI : OnMessage<CurrentThemeChanged>
{
    [SerializeField] private CurrentTheme currentTheme;
    [SerializeField] private string elementCategoryName;
    [SerializeField] private TextMeshProUGUI[] txts;
    
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
        txts.ForEach(t => t.color = c);
    }
}
