using System;
using I2.Loc;
using TMPro;
using UnityEngine;

public class SingleHeroSelectionUiPresenter : MonoBehaviour
{
    [SerializeField] private Localize promptLabel;
    [SerializeField] private HeroDisplayPresenter[] presenters;

    public SingleHeroSelectionUiPresenter Initialized(GetUserSelectedHero msg, Action onFinished)
    {
        promptLabel.SetTerm(msg.PromptTerm);
        for (var i = 0; i < presenters.Length; i++)
        {
            var capturedIndex = i;
            var hasOption = msg.Options.Length > i;
            presenters[i].gameObject.SetActive(hasOption);
            if (hasOption)
            {
                var option = msg.Options[i];
                presenters[i].Init(option, true, () =>
                {
                    presenters[capturedIndex].DisableClick();
                    HideAllExcept(capturedIndex);
                    msg.OnSelected(option);
                    onFinished();
                });
            }
        }
        return this;
    }

    private void HideAllExcept(int index)
    {
        for (var i = 0; i < presenters.Length; i++)
            if (i != index)
                presenters[i].Hide();
    }
}
