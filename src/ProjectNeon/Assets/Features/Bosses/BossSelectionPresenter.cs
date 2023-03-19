using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class BossSelectionPresenter : MonoBehaviour
{
    [SerializeField] private Image bust;
    [SerializeField] private Localize name;
    [SerializeField] private Localize description;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private AllBosses bosses;

    private int _index;
    
    public Boss SelectedBoss { get; private set; }

    private void Awake()
    {
        nextButton.onClick.AddListener(() => RenderBoss(_index == bosses.bosses.Length - 1 ? 0 : _index + 1));
        prevButton.onClick.AddListener(() => RenderBoss(_index == 0 ? bosses.bosses.Length - 1 : _index - 1));
        RenderBoss(Rng.Int(0, bosses.bosses.Length));
    }

    private void RenderBoss(int index)
    {
        _index = index;
        SelectedBoss = bosses.bosses[_index];
        bust.sprite = SelectedBoss.Bust;
        name.SetTerm(SelectedBoss.NameTerm);
        description.SetTerm(SelectedBoss.DescriptionTerm);
    }
}