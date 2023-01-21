using System.Linq;
using UnityEngine;

public class LanguageSelectionPresenter : MonoBehaviour
{
    [SerializeField] private LanguageButtonPresenter[] optionButtons;
    [SerializeField] private LanguageOption[] options;

    void Start()
    {
        var enabledOptions = options.Where(o => o.Enabled).ToArray();
        for (var i = 0; i < optionButtons.Length; i++)
        {
            var btn = optionButtons[i];
            if (enabledOptions.Length > i)
            {
                var option = enabledOptions[i];
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
