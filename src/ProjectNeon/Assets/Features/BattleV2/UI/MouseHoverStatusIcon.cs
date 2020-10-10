
using UnityEngine;

public class MouseHoverStatusIcon
{
    private Maybe<WorldStatusIconPresenter> _last = Maybe<WorldStatusIconPresenter>.Missing();
    
    public void Update(Maybe<WorldStatusIconPresenter> icon)
    {
        if (icon.IsMissing && _last.IsMissing)
            return;
        
        if (icon.IsPresent && _last.Value != icon.Value)
        {
            Debug.Log($"Show Tooltip for {icon.Value.name}");
            _last = icon.Value;
            _last.Value.ShowTooltip();
        }
        else if (icon.IsMissing)
        {
            Debug.Log($"Hide Tooltip");
            _last = Maybe<WorldStatusIconPresenter>.Missing();
            Message.Publish(new HideTooltip());
        }
    }
}
