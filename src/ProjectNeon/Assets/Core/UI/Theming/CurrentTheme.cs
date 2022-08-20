using UnityEngine;

[CreateAssetMenu(menuName = "Theme/CurrentTheme")]
public class CurrentTheme : ScriptableObject
{
    [SerializeField] private CustomTheme theme;

    public CustomTheme Value => theme;
    
    public void Set(CustomTheme t)
    {
        theme = t;
        Message.Publish(new CurrentThemeChanged());
    }

    public void SetIfNonePresent(CustomTheme t)
    {
        if (theme == null)
            Set(t);
    }
}
