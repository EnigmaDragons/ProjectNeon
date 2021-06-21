using UnityEngine;

public class InfoDialogUiController : OnMessage<ShowInfoDialog>
{
    [SerializeField] private InfoDialogPresenter presenter;

    protected override void Execute(ShowInfoDialog msg) => presenter.Show(msg);
}
