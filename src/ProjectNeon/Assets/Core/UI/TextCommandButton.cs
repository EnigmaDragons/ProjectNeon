using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class TextCommandButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

    private readonly Color _disabledTextColor = new Color(0.7f, 0.7f, 0.7f);
    private Button _button;
    private Action _cmd = () => { };
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
        _button.interactable = !isDisabled;
        label.color = isDisabled ? _disabledTextColor : activeColor; 
    }

    public TextCommandButton Initialized(NamedCommand cmd)
    {
        Init(cmd);
        return this;
    }

    public void Init(string commandText, Action cmd)
    {
        InitButton();
        label.text = GetLocalizedStringOrDefault(commandText);
        _cmd = cmd;
        _button.interactable = true;
        gameObject.SetActive(true);
    }

    public void InitRaw(string rawCommand, Action cmd)
    {
        InitButton();
        label.text = rawCommand;
        _cmd = cmd;
        _button.interactable = true;
        gameObject.SetActive(true);
    }
    
    public void Init(NamedCommand cmd) => Init(cmd.Name, cmd.Action.Invoke);

    public void Select() => _button.Select();
    public void Execute() => _button.onClick.Invoke();

    private string GetLocalizedStringOrDefault(string commandText)
    {
        var localized = LocalizationSettings.StringDatabase.GetLocalizedString("UI", commandText);
        return string.IsNullOrWhiteSpace(localized) ? commandText : localized;
    }
}
