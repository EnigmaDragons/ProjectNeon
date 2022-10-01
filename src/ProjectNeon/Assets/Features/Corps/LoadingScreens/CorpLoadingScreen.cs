using UnityEngine;

[CreateAssetMenu(menuName = "LoadingScreen/Corp")]
public class CorpLoadingScreen : ScriptableObject
{
    [SerializeField] private StaticCorp corp;
    [SerializeField] private Sprite image;
    [SerializeField] private string locationTitle;
    [SerializeField] private Color titleColor;

    public StaticCorp Corp => corp;
    public Sprite Image => image;
    public string LocationTitle => string.IsNullOrWhiteSpace(locationTitle) ? corp.Name : locationTitle;
    public Color LocationTitleColor => titleColor == Color.black ? corp.Color1 : titleColor;
}
