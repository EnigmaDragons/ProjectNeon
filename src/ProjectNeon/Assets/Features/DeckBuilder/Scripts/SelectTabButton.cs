using UnityEngine;
using UnityEngine.UI;

public class SelectTabButton : OnMessage<CustomizationTabSwitched>
{
    [SerializeField] private string tabName;
    [SerializeField] private Image selected;
    [SerializeField] private Button button;
    [SerializeField] private bool startsActive;
    [SerializeField] private GameObject tab;

    private bool _selected;
    
    private void Awake()
    {
        _selected = startsActive;
        tab.SetActive(_selected);
        selected.gameObject.SetActive(startsActive);
        button.onClick.AddListener(() =>
        {
            if (!_selected)
                Message.Publish(new CustomizationTabSwitched { TabName = tabName });
        });
    }
    
    protected override void Execute(CustomizationTabSwitched msg)
    {
        _selected = msg.TabName == tabName;
        selected.gameObject.SetActive(_selected);
        tab.SetActive(_selected);
    }
}