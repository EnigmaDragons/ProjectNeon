using I2.Loc;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class LocalizedCommandButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private Localize label;

    private readonly Color _disabledTextColor = new Color(0.7f, 0.7f, 0.7f);
    private Button _button;
    private Action _cmd = () => { };
    private Action _onHoverEnter = () => { };
    private Action _onHoverExit = () => { };
    private bool _isInitialized;

    private void Awake() => InitButton();

    private void InitButton()
    {
        if (_isInitialized)
            return;
        
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => _cmd());
        _isInitialized = true;
    }

    public void Hide() => gameObject.SetActive(false);
    public void SetButtonDisabled(bool isDisabled, Color activeColor)
    {
        if (!_isInitialized)
            InitButton();
        
        _button.interactable = !isDisabled;
        labelText.color = isDisabled ? _disabledTextColor : activeColor; 
    }

    public LocalizedCommandButton Initialized(NamedCommand cmd)
    {
        Init(cmd);
        return this;
    }

    public void Init(Action cmd)
    {        
        InitButton();
        _cmd = cmd;
        _button.interactable = true;
        gameObject.SetActive(true);
    }
    
    public void InitTerm(string term, Action cmd)
    {
        InitButton();
        label.SetTerm(term);
        _cmd = cmd;
        _button.interactable = true;
        gameObject.SetActive(true);
    }

    public void InitFinalText(string rawCommandText, Action cmd)
    {
        InitButton();
        label.SetFinalText(rawCommandText);
        _cmd = cmd;
        _button.interactable = true;
        gameObject.SetActive(true);
    }

    public void SetHoverActions(Action onHoverEnter, Action onHoverExit)
    {
        _onHoverEnter = onHoverEnter;
        _onHoverExit = onHoverExit;
    }
    
    public void Init(NamedCommand cmd) => InitTerm(cmd.Name, cmd.Action.Invoke);
    
    public void Select() => _button.Select();
    public void Execute() => _button.onClick.Invoke();

    public void OnHoverEnter() => _onHoverEnter();
    public void OnHoverExit() => _onHoverExit();
}
