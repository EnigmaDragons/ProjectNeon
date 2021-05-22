using System;
using UnityEngine;

public class EquipmentPicker : OnMessage<GetUserSelectedEquipment>
{
    [SerializeField] private GameObject view;
    [SerializeField] private TextCommandButton noneButton;
    [SerializeField] private EquipmentPresenter equipmentPrototype;
    [SerializeField] private GameObject optionsParent;

    private Action<Maybe<Equipment>> _onSelected = _ => { };

    private void Awake()
    {
        noneButton.Init("None", SelectNone);
    }
    
    protected override void Execute(GetUserSelectedEquipment msg)
    {
        _onSelected = msg.OnSelected;
        ClearView();
        msg.Options.ForEach(o =>
            Instantiate(equipmentPrototype, optionsParent.transform)
                .Initialized(o, () => SelectEquipment(o)));
        view.SetActive(true);
    }

    private void ClearView() => optionsParent.DestroyAllChildren();
    private void SelectEquipment(Equipment e)
    {
        Debug.Log($"Selected Equipment {e.Name}");
        FinishSelection(() => _onSelected(new Maybe<Equipment>(e)));
    }

    private void SelectNone() => FinishSelection(() => _onSelected(Maybe<Equipment>.Missing()));
    private void FinishSelection(Action s)
    {
        view.SetActive(false);
        s();
    }
}
