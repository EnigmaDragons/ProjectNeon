using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CorpWorldArtPresenter : MonoBehaviour
{
    [SerializeField] private AllCorpLoadingScreens allCorps;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image[] logoImages;
    [SerializeField] private TextMeshProUGUI corpNameLabel;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    private IndexSelector<CorpLoadingScreen> _art;
    
    private void Awake()
    {
        var allArt = allCorps.All;
        _art = new IndexSelector<CorpLoadingScreen>(allArt, Rng.Int(allArt.Length));
        Display(_art.Current);
        nextButton.onClick.AddListener(DisplayNext);
        prevButton.onClick.AddListener(DisplayPrev);
    }

    public void DisplayNext() => Display(_art.MoveNext());
    public void DisplayPrev() => Display(_art.MovePrevious());

    private void Display(CorpLoadingScreen screen)
    {
        backgroundImage.sprite = screen.Image;
        var corp = screen.Corp;
        logoImages.ForEach(l => l.sprite = corp.Logo);
        corpNameLabel.text = screen.LocationTitle;
        corpNameLabel.color = screen.LocationTitleColor;
    }
}
