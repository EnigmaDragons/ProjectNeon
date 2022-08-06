using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayRandomCorpLoadingScreen : MonoBehaviour
{
    [SerializeField] private AllCorpLoadingScreens allCorps;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image[] logoImages;
    [SerializeField] private TextMeshProUGUI corpNameLabel;

    private void Awake() => DisplayNext();
    
    public void DisplayNext() => Display(allCorps.GetRandomNext());

    private void Display(CorpLoadingScreen screen)
    {
        backgroundImage.sprite = screen.Image;
        var corp = screen.Corp;
        logoImages.ForEach(l => l.sprite = corp.Logo);
        corpNameLabel.text = screen.LocationTitle;
        corpNameLabel.color = screen.LocationTitleColor;
    }
}
