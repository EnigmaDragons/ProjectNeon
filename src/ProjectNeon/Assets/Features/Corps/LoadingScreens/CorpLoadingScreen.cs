using UnityEngine;

[CreateAssetMenu(menuName = "LoadingScreen/Corp")]
public class CorpLoadingScreen : ScriptableObject, ILocalizeTerms
{
    [SerializeField] public int id;
    [SerializeField] private StaticCorp corp;
    [SerializeField] private Sprite image;
    [SerializeField] public string locationTitle;
    [SerializeField] private Color titleColor;

    public StaticCorp Corp => corp;
    public Sprite Image => image;
    public Color LocationTitleColor => titleColor == Color.black ? corp.Color1 : titleColor;
    public string Term => $"LoadingScreens/LocationTitle{id}";

    public string[] GetLocalizeTerms()
        => new[] {Term};
}
