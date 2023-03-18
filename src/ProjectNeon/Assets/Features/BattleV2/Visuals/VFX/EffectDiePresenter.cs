using DG.Tweening;
using TMPro;
using UnityEngine;

public class EffectDiePresenter : OnMessage<DieRolled, ShowDieResult>
{
    [SerializeField] private FloatReference flyInSeconds;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI dieRollLabel;
    [SerializeField] private FloatReference dieRollDuration;
    [SerializeField] private FloatReference showDiceDuration;
    [SerializeField] private AudioClipVolume rollSound;
    [SerializeField] private AudioClipVolume dieShakeSound;
    [SerializeField] private UiSfxPlayer player;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject die;

    private Vector3 _initialPos;
    private int _dieResult;
    private float _remainingRollDuration;
    private float _remainingShowDuration;
    private bool _rollInProgress;

    private void Awake()
    {
        _initialPos = transform.position;
        Hide();
    }

    protected override void Execute(DieRolled dieRoll) => _dieResult = dieRoll.Number;
    
    protected override void Execute(ShowDieResult msg)
    {
        Hide();
        FlyIn();
        _rollInProgress = true;
        _remainingRollDuration = dieRollDuration.Value;
        _remainingShowDuration = showDiceDuration.Value;
        Message.Publish(new DieRollShaking(transform));
        player.Play(dieShakeSound);
        panel.SetActive(true);
    }

    private void FlyIn()
    {
        transform.DOKill();
        transform.position.Set(-3000, transform.position.y, transform.position.z);
        transform.DOMove(_initialPos, flyInSeconds);
    }
    
    private void Hide()
    {
        transform.DOKill();
        panel.SetActive(false);
        _rollInProgress = false;
    }

    private void Update()
    {
        if (!_rollInProgress)
            return;

        if (_remainingRollDuration > 0f)
        {
            dieRollLabel.text = Rng.Int(1, 20).ToString();
            _remainingRollDuration -= Time.deltaTime;
            die.transform.rotation = Quaternion.Euler(0, 0, Rng.Int(0, 360));

            if (_remainingRollDuration > 0f) 
                return;

            dieRollLabel.text = _dieResult.ToString();
            die.transform.rotation = Quaternion.identity;
            player.Play(rollSound);
        }
        else
        {
            _remainingShowDuration -= Time.deltaTime;
            if (_remainingShowDuration > 0f)
                return;
            Hide();
        }
    }
}