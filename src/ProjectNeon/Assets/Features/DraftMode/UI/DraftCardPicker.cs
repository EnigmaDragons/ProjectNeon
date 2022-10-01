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
            var s = slots[i];
            s.Set(e, () => SelectCard(e, s.transform));
            s.SetHoverAction(() =>
            {
                Message.Publish(new CardHovered(s.transform));
                s.SetHoverHighlight(true);
            }, () => s.SetHoverHighlight(false));
        }
        panel.SetActive(true);
        Log.Info("Begun Card Pick");
    }
    
    private void SelectCard(Card c, Transform t)
    {        
        if (_hasSelected)
            return;

        _hasSelected = true;
        HideView();
        Debug.Log($"Draft - Selected Card {c.Name}");
        Message.Publish(new DraftItemPicked(t));
        _onSelected(new Maybe<Card>(c));
    }
    
    private void HideView() => panel.SetActive(false);
}
