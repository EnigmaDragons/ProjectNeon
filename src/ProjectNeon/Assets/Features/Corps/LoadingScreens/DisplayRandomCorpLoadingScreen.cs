using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayRandomCorpLoadingScreen : MonoBehaviour
{
    [SerializeField] private AllCorpLoadingScreens allCorps;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image[] logoImages;
    [SerializeField] private TextMeshProUGUI corpNameLabel;
    [SerializeField] private Localize corpNameLocalize;

    private void Awake() => DisplayNext();
    
    public void DisplayNext() => Display(allCorps.GetRandomNext());

    private void Display(CorpLoadingScreen screen)
    {
        backgroundImage.sprite = screen.Image;
        var corp = screen.Corp;
        logoImages.ForEach(l => l.sprite = corp.Logo);
        corpNameLocalize.SetTerm(screen.Term);
        corpNameLabel.color = screen.LocationTitleColor;
    }
}
