using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CorpDescriptionsPresenter : MonoBehaviour
{
    [SerializeField] private AllCorps corps;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private CorpBrandingPresenter corpBranding;
    [SerializeField] private TextMeshProUGUI corpDescriptionLabel;

    private IndexSelector<Corp> _allCorps;
    
    private void Awake()
    {
        _allCorps = new IndexSelector<Corp>(corps.GetCorps());
        Render(_allCorps.Current);
        nextButton.onClick.AddListener(MoveNext);
        prevButton.onClick.AddListener(MovePrev);
    }

    private void MoveNext() => Render(_allCorps.MoveNext());
    private void MovePrev() => Render(_allCorps.MovePrevious());
    
    private void Render(Corp corp)
    {
        corpBranding.Init(corp);
        corpDescriptionLabel.text = corp.LongDescription;
    }
}
