using System;
using UnityEngine;

public class DraftGearPicker : OnMessage<GetUserSelectedEquipmentForDraft>
{
    [SerializeField] private GameObject panel;
    [SerializeField] private EquipmentPresenter[] slots;
    
    private Action<Maybe<Equipment>> _onSelected = _ => { };
    private bool _hasSelected = false;
    
    protected override void Execute(GetUserSelectedEquipmentForDraft msg)
    {
        _hasSelected = false;
        _onSelected = msg.OnSelected;
        HideView();
        
        if (msg.Options.Length != slots.Length)
        {
            Log.Error($"Draft Gear Picker has {slots.Length}, but number of options offered is {msg.Options.Length}");
            return;
        }

        for (var i = 0; i < slots.Length; i++)
        {
            var e = msg.Options[i];
            slots[i].Set(e, () =>  SelectEquipment(e));
        }
        panel.SetActive(true);
    }
    
    private void SelectEquipment(Equipment e)
    {
        if (_hasSelected)
            return;

        _hasSelected = true;
        HideView();
        Debug.Log($"Draft - Selected Equipment {e.Name}");
        _onSelected(new Maybe<Equipment>(e));
    }

    private void HideView() => panel.SetActive(false);
}
