using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class TextCommandButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

    private readonly Color _disabledTextColor = new Color(192, 192, 192);
    private Color _activeTextColor;
    private Button _button;
    private Action _cmd = () => { };

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => _cmd());
        _activeTextColor = label.color;
    }

    public void Hide() => gameObject.SetActive(false);
    public void SetButtonDisabled(bool isDisabled)
    {
        _button.interactable = !isDisabled;
        label.color = _disabledTextColor;
    }

    public TextCommandButton Initialized(NamedCommand cmd)
    {
        Init(cmd);
        return this;
    }

    public void Init(string commandText, Action cmd)
    {
        label.text = commandText;
        label.color = _activeTextColor;
        _cmd = cmd;
        gameObject.SetActive(true);
    }
    
    public void Init(NamedCommand cmd) => Init(cmd.Name, cmd.Action.Invoke);

    public void Select() => _button.Select();
    public void Execute() => _button.onClick.Invoke();
}
