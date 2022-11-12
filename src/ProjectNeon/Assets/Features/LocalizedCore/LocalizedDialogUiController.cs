using UnityEngine;

public class LocalizedDialogUiController : OnMessage<ShowLocalizedDialog, HideLocalizedDialog>
{
    [SerializeField] private LocalizedDialogPresenter target;

    protected override void Execute(ShowLocalizedDialog msg) => target.Show(msg);

    protected override void Execute(HideLocalizedDialog msg) => target.Hide();
}
