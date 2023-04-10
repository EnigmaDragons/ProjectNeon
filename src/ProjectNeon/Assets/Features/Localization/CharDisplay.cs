#if UNITY_EDITOR
using System;
using System.Text;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class CharDisplay : MonoBehaviour
{
    [SerializeField] private Localize characterContainer;
    [SerializeField] private Button button;
    [SerializeField] private Image image;

    private char _character;
    private bool _selected;
    private Action<bool, char> _onSelect;

    public void Init(char character, bool selected, Action<bool, char> onSelect)
    {
        _character = character;
        _selected = selected;
        _onSelect = onSelect;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            _selected = !_selected;
            _onSelect(_selected, _character);
            UpdateButtonColor();
        });
        UpdateButtonColor();
        characterContainer.SetFinalText(character.ToString());
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void UpdateButtonColor()
    {
        image.color = _selected ? Color.yellow : Color.white;
    }
}
#endif