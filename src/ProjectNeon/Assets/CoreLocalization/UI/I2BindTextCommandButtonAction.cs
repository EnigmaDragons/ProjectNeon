using System;
using UnityEngine;
using UnityEngine.Events;

public class I2BindTextCommandButtonAction : MonoBehaviour, ILocalizeTerms
{
    [SerializeField] private LocalizedCommandButton btn;
    [SerializeField] private string term;
    [SerializeField] private UnityEvent action;
    [SerializeField] private ButtonSounds sounds;
    [SerializeField] private FModButtonSounds fmodSounds;
    [SerializeField] private bool isPrimaryActionButton;

    private void Awake()
    {
        btn.InitTerm(term, () => action.Invoke());
        if (sounds != null && isPrimaryActionButton)
            sounds.SetAsPrimary();
        if (fmodSounds != null && isPrimaryActionButton)
            fmodSounds.SetAsPrimary();
    }

    public string[] GetLocalizeTerms()
        => new[] {term};
}