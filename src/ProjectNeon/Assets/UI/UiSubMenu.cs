using UnityEngine;

public class UiSubMenu : OnMessage<CloseUiSubMenus>
{
    protected override void Execute(CloseUiSubMenus msg)
    {
        this.ExecuteAfterDelay(() => gameObject.SetActive(false), 0.1f);
    }
}