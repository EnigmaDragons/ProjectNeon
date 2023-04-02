using System;
using UnityEngine;

public class EquipmentPicker : OnMessage<GetUserSelectedEquipment>, ILocalizeTerms
{
    [SerializeField] private GameObject view;
    [SerializeField] private LocalizedCommandButton noneButton;
    [SerializeField] private EquipmentPresenter equipmentPrototype;
    [SerializeField] private GameObject optionsParent;

    private Action<Maybe<StaticEquipment>> _onSelected = _ => { };

    private const string NoneTerm = "Menu/None";
    
    private void Awake()
    {
        noneButton.InitTerm(NoneTerm, SelectNone);
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
    private void SelectEquipment(StaticEquipment e)
    {
        Debug.Log($"Selected Equipment {e.Name}");
        FinishSelection(() => _onSelected(new Maybe<StaticEquipment>(e)));
    }

    private void SelectNone() => FinishSelection(() => _onSelected(Maybe<StaticEquipment>.Missing()));
    private void FinishSelection(Action s)
    {
        view.SetActive(false);
        s();
    }

    public string[] GetLocalizeTerms()
        => new[] {NoneTerm};
}
