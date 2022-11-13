using UnityEngine;

public class LanguageSelectionPresenter : MonoBehaviour
{
    [SerializeField] private LanguageButtonPresenter[] optionButtons;
    [SerializeField] private LanguageOption[] options;

    void Start()
    {
        for (var i = 0; i < optionButtons.Length; i++)
        {
            var btn = optionButtons[i];
            if (options.Length > i)
            {
                var option = options[i];
                btn.Init(option, () =>
                {
                    option.Select();
                    Message.Publish(new HideNamedTarget("LanguageSelectionUI"));
                });
            }
            else
                btn.gameObject.SetActive(false);
        }
    }
}
