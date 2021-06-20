
using UnityEngine;

public class TwoChoiceDialogUiController : OnMessage<ShowTwoChoiceDialog, HideTwoChoiceDialog>
{
    [SerializeField] private TwoChoiceDialogPresenter target;

    protected override void Execute(ShowTwoChoiceDialog msg) => target.Show(msg);

    protected override void Execute(HideTwoChoiceDialog msg) => target.Hide();
}