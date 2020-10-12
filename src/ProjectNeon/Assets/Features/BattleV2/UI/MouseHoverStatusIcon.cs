
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
            _last = icon.Value;
            _last.Value.ShowTooltip();
        }
        else if (icon.IsMissing)
        {
            _last = Maybe<WorldStatusIconPresenter>.Missing();
            Message.Publish(new HideTooltip());
        }
    }
}
