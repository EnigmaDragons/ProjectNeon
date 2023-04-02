using System;
using UnityEngine;

public class DraftGearPicker : OnMessage<GetUserSelectedEquipmentForDraft>
{
    [SerializeField] private GameObject panel;
    [SerializeField] private EquipmentPresenter[] slots;
    [SerializeField] private Canvas hoverCardOverrideCanvas;
    
    private Action<Maybe<StaticEquipment>> _onSelected = _ => { };
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
            var itemTransform = slots[i].transform;
            var s = slots[i];
            s.Set(e, () =>  SelectEquipment(e, itemTransform));
            s.WithHoverSettings(true, true, false);
            if (hoverCardOverrideCanvas != null)
                s.SetReferencedCardCanvas(hoverCardOverrideCanvas);
        }
        panel.SetActive(true);
    }
    
    private void SelectEquipment(StaticEquipment e, Transform itemTransform)
    {
        if (_hasSelected)
            return;

        _hasSelected = true;
        HideView();
        Debug.Log($"Draft - Selected Equipment {e.Name}");
        Message.Publish(new DraftItemPicked(itemTransform));
        _onSelected(new Maybe<StaticEquipment>(e));
    }

    private void HideView() => panel.SetActive(false);
}
