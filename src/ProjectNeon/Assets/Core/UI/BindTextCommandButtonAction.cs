
using UnityEngine;
using UnityEngine.Events;

public class BindTextCommandButtonAction : MonoBehaviour
{
    [SerializeField] private TextCommandButton btn;
    [SerializeField] private string text;
    [SerializeField] private UnityEvent action;
    [SerializeField] private ButtonSounds sounds;
    [SerializeField] private bool isPrimaryActionButton;

    private void Awake()
    {
        btn.Init(text, () => action.Invoke());
        if (sounds != null && isPrimaryActionButton)
            sounds.SetAsPrimary();
    }
}
