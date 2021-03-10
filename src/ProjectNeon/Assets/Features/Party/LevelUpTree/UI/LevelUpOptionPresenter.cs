using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class LevelUpOptionPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button button;

    private HeroLevelUpOption _option;

    private void Awake()
    {
        if (button != null)
            button.onClick.AddListener(SelectLevelUpOption);
    }
    
    public LevelUpOptionPresenter Initialized(HeroLevelUpOption o)
    {
        _option = o;
        text.text = o.Description;
        return this;
    }

    private void SelectLevelUpOption()
    {
        if (_option != null)
            Message.Publish(new LevelUpOptionSelected(_option));
    }
}
