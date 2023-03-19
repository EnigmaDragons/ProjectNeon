using UnityEngine;
using UnityEngine.UI;

public class DeckBuilderButton : OnMessage<StartBattleInitiated>
{
    [SerializeField] private Button button;
    
    protected override void Execute(StartBattleInitiated msg)
        => button.interactable = false;
}