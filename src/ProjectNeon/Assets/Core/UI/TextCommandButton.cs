using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class TextCommandButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

    private Button _button;

    private void Awake() => _button = GetComponent<Button>();

    public TextCommandButton Initialized(NamedCommand cmd)
    {
        Init(cmd);
        return this;
    }

    public void Init(string commandText, Action cmd)
    {
        label.text = commandText;
        _button.onClick.AddListener(() => cmd.Invoke());
    }
    
    public void Init(NamedCommand cmd)
    {
        label.text = cmd.Name;
        _button.onClick.AddListener(() => cmd.Action.Invoke());
    }

    public void Select() => _button.Select();
    public void Execute() => _button.onClick.Invoke();
}
