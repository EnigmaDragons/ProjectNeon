using System;
using TMPro;
using UnityEngine;

public sealed class VictoryEquipmentSelectionUI : OnMessage<GetUserSelectedEquipment>
{
    [SerializeField] private GameObject view;
    [SerializeField] private EquipmentPresenter equipmentPresenter;
    [SerializeField] private GameObject optionsParent;
    [SerializeField] private BattleState state;
    [SerializeField] private TextMeshProUGUI creditsLabel;

    private Action<Maybe<Equipment>> _onSelected = _ => { };
    private void ClearView() => optionsParent.DestroyAllChildren();
    private bool _hasSelected = false;

    protected override void Execute(GetUserSelectedEquipment msg)
    {
        _hasSelected = false;
        _onSelected = msg.OnSelected;
        ClearView();
        msg.Options.ForEach(o =>
            Instantiate(equipmentPresenter, optionsParent.transform)
                .Set(o, () => SelectEquipment(o)));
        creditsLabel.text = state.RewardCredits.ToString();
        view.SetActive(true);
    }

    private void SelectEquipment(Equipment e)
    {
        if (_hasSelected)
            return;

        _hasSelected = true;
        Debug.Log($"Selected Equipment {e.Name}");
        _onSelected(new Maybe<Equipment>(e));
        ClearView();
        Instantiate(equipmentPresenter, optionsParent.transform).Set(e, () => { });
    }
}