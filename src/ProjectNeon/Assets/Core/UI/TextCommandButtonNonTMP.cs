using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TextCommandButtonNonTMP : MonoBehaviour
{
    [SerializeField] private Text label;

    private Button _button;

    private void Awake() => InitButton();

    public TextCommandButtonNonTMP Initialized(NamedCommand cmd)
    {
        Init(cmd);
        return this;
    }

    public void Init(string commandText, Action cmd)
    {
        InitButton();
        
        label.text = commandText;
        _button.onClick.AddListener(cmd.Invoke);
    }
    
    public void Init(NamedCommand cmd)
    {        
        InitButton();
        
        label.text = cmd.Name;
        _button.onClick.AddListener(() => cmd.Action.Invoke());
    }

    private void InitButton()
    {
        if (_button == null)
            _button = GetComponent<Button>();
    }

    public void Select() => _button.Select();
    public void Execute() => _button.onClick.Invoke();
}
