
using UnityEngine;
using UnityEngine.Events;

public class BindTextCommandButtonAction : MonoBehaviour
{
    [SerializeField] private TextCommandButton btn;
    [SerializeField] private string text;
    [SerializeField] private UnityEvent action;
    [SerializeField] private ButtonSounds sounds;
    [SerializeField] private FModButtonSounds fmodSounds;
    [SerializeField] private bool isPrimaryActionButton;
    [SerializeField] private bool ignoreText = false;

    private void Awake()
    {
        if (ignoreText)
            btn.Init(() => action.Invoke());
        else
            btn.Init(text, () => action.Invoke());
        if (sounds != null && isPrimaryActionButton)
            sounds.SetAsPrimary();
        if (fmodSounds != null && isPrimaryActionButton)
            fmodSounds.SetAsPrimary();
    }
}
