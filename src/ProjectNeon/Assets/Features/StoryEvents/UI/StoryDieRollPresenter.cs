using DG.Tweening;
using TMPro;
using UnityEngine;

public class StoryDieRollPresenter : OnMessage<ShowDieRoll, ShowNoDieRollNeeded, HideDieRoll>
{
    [SerializeField] private GameObject noDieRollNeededPanel;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject die;
    [SerializeField] private TextMeshProUGUI actionLabel;
    [SerializeField] private TextMeshProUGUI dieRollLabel;
    [SerializeField] private FloatReference dieRollDuration;
    [SerializeField] private AudioClipVolume rollSound;
    [SerializeField] private AudioClipVolume dieShakeSound;
    [SerializeField] private UiSfxPlayer player;

    private Vector3 initialPos;
    private string dieResult;
    private float remainingRollDuration;
    private bool rollInProgress;
    
    private void Awake()
    {
        initialPos = transform.position;
        Hide();
    }
    
    protected override void Execute(ShowDieRoll msg)
    {
        dieResult = msg.Roll.ToString();
        actionLabel.text = "Rolling";
        Hide();
        FlyIn(0.3f);
        rollInProgress = true;
        remainingRollDuration = dieRollDuration.Value;
        Message.Publish(new DieRollShaking(transform));
        player.Play(dieShakeSound);
        panel.SetActive(true);
    }

    protected override void Execute(ShowNoDieRollNeeded msg)
    {
        Hide();
        FlyIn(0.5f);
        noDieRollNeededPanel.SetActive(true);
    }

    protected override void Execute(HideDieRoll msg) => Hide();

    private void FlyIn(float duration)
    {
        transform.DOKill();
        transform.position.Set(-3000, transform.position.y, transform.position.z);
        transform.DOMove(initialPos, duration);
    }
    
    private void Hide()
    {
        transform.DOKill();
        noDieRollNeededPanel.SetActive(false);
        panel.SetActive(false);
        rollInProgress = false;
    }

    private void Update()
    {
        if (!rollInProgress)
            return;

        dieRollLabel.text = Rng.Int(1, 20).ToString();
        remainingRollDuration -= Time.unscaledDeltaTime;
        die.transform.rotation = Quaternion.Euler(0, 0, Rng.Int(0, 360));

        if (!(remainingRollDuration <= 0f)) 
            return;

        actionLabel.text = "You Rolled";
        dieRollLabel.text = dieResult;
        rollInProgress = false;
        die.transform.rotation = Quaternion.identity;
        Message.Publish(new DieThrown(transform));
        player.Play(rollSound);
    }
}
