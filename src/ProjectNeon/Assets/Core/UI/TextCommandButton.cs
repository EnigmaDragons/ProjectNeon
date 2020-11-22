using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class TextCommandButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

    private readonly Color _disabledTextColor = new Color(0.7f, 0.7f, 0.7f);
    private Button _button;
    private Action _cmd = () => { };

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => _cmd());
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
        _button = GetComponent<Button>();
        label.text = commandText;
        _cmd = cmd;
        _button.interactable = true;
        gameObject.SetActive(true);
    }
    
    public void Init(NamedCommand cmd) => Init(cmd.Name, cmd.Action.Invoke);

    public void Select() => _button.Select();
    public void Execute() => _button.onClick.Invoke();
}
