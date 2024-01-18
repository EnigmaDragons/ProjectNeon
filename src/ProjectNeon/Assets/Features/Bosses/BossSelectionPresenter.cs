using System.Linq;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class BossSelectionPresenter : MonoBehaviour
{
    [SerializeField] private Image bust;
    [SerializeField] private Localize name;
    [SerializeField] private Localize description;
    [SerializeField] private AllBosses bosses;

    private Boss[] _bossesAvailable;

    private int _index;
    
    public Boss SelectedBoss { get; private set; }

    public void Next() => RenderBoss(_index == _bossesAvailable.Length - 1 ? 0 : _index + 1);

    private void Awake()
    {
        if (CurrentProgressionData.Data.HasSeenAlgeronFinalBoss)
            _bossesAvailable = bosses.bosses;
        else
            _bossesAvailable = bosses.bosses.Where(x => x.id != 2).ToArray(); //algeron's id
        RenderBoss(Rng.Int(0, _bossesAvailable.Length));
    }

    private void RenderBoss(int index)
    {
        _index = index;
        SelectedBoss = _bossesAvailable[_index];
        bust.sprite = SelectedBoss.Bust;
        name.SetTerm(SelectedBoss.NameTerm);
        description.SetTerm(SelectedBoss.DescriptionTerm);
    }
}