using System;
using TMPro;
using UnityEngine;

public class SingleHeroSelectionUiPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptLabel;
    [SerializeField] private HeroDisplayPresenter[] presenters;

    public SingleHeroSelectionUiPresenter Initialized(GetUserSelectedHero msg, Action onFinished)
    {
        promptLabel.text = msg.Prompt;
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
