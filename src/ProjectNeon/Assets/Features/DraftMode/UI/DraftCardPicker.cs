using System;
using UnityEngine;

public class DraftCardPicker : OnMessage<GetUserSelectedCardForDraft>
{
    [SerializeField] private GameObject panel;
    [SerializeField] private CardPresenter[] slots;
    
    private Action<Maybe<Card>> _onSelected = _ => { };
    private bool _hasSelected = false;
    
    protected override void Execute(GetUserSelectedCardForDraft msg)
    {
        _hasSelected = false;
        _onSelected = msg.OnSelected;
        HideView();
        
        if (msg.Options.Length != slots.Length)
        {
            Log.Error($"Draft Card Picker has {slots.Length}, but number of options offered is {msg.Options.Length}");
            return;
        }

        for (var i = 0; i < slots.Length; i++)
        {
            var e = msg.Options[i];
            slots[i].Set(e, () =>  SelectCard(e));
        }
        panel.SetActive(true);
        Log.Info("Begun Card Pick");
    }
    
    private void SelectCard(Card c)
    {        
        if (_hasSelected)
            return;

        _hasSelected = true;
        HideView();
        Debug.Log($"Draft - Selected Card {c.Name}");
        _onSelected(new Maybe<Card>(c));
    }
    
    private void HideView() => panel.SetActive(false);
}
