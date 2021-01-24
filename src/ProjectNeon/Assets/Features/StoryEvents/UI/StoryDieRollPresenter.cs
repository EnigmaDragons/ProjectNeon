using DG.Tweening;
using TMPro;
using UnityEngine;

public class StoryDieRollPresenter : OnMessage<ShowDieRoll, ShowNoDieRollNeeded, HideDieRoll>
{
    [SerializeField] private GameObject noDieRollNeededPanel;
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI dieRollLabel;

    private Vector3 initialPos;
    
    private void Awake()
    {
        initialPos = transform.position;
        Hide();
    }
    
    protected override void Execute(ShowDieRoll msg)
    {
        dieRollLabel.text = msg.Roll.ToString();
        Hide();
        FlyIn();
        panel.SetActive(true);
    }

    protected override void Execute(ShowNoDieRollNeeded msg)
    {
        Hide();
        FlyIn();
        noDieRollNeededPanel.SetActive(true);
    }

    protected override void Execute(HideDieRoll msg) => Hide();

    private void FlyIn()
    {
        transform.DOKill();
        transform.position.Set(-3000, transform.position.y, transform.position.z);
        transform.DOMove(initialPos, 0.5f);
    }
    
    private void Hide()
    {
        transform.DOKill();
        noDieRollNeededPanel.SetActive(false);
        panel.SetActive(false);
    }
}
