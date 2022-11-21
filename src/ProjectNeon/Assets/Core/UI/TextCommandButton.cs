using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button)), IgnoreForLocalization]
public sealed class TextCommandButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

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
        label.color = isDisabled ? _disabledTextColor : activeColor; 
    }

    public TextCommandButton Initialized(NamedCommand cmd)
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
    
    public void Init(string commandText, Action cmd)
    {
        InitButton();
        label.text = GetLocalizedStringOrDefault(commandText);
        _cmd = cmd;
        _button.interactable = true;
        gameObject.SetActive(true);
    }

    public void SetHoverActions(Action onHoverEnter, Action onHoverExit)
    {
        _onHoverEnter = onHoverEnter;
        _onHoverExit = onHoverExit;
    }
    
    public void Init(NamedCommand cmd) => Init(cmd.Name, cmd.Action.Invoke);
    
    public void Select() => _button.Select();
    public void Execute() => _button.onClick.Invoke();

    private string GetLocalizedStringOrDefault(string commandText)
    {
        return commandText;
    }

    public void OnHoverEnter() => _onHoverEnter();
    public void OnHoverExit() => _onHoverExit();
}
