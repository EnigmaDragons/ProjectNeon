using System.Collections.Generic;
using UnityEngine;

public class BattleUiVisibilityController : OnMessage<SetBattleUiElementVisibility>
{
    [SerializeField] private NamedGameObject[] uiElements;

    private Dictionary<string, GameObject> _uiElements;

    private void Awake()
    {
        _uiElements = uiElements.SafeToDictionary(u => u.Name, u => u.Obj);
    }
    
    protected override void Execute(SetBattleUiElementVisibility msg)
    {
        if (_uiElements.TryGetValue(msg.UiElementName, out var el))
            el.SetActive(msg.ShouldShow);
        if (msg.UiElementName == BattleUiElement.EnemyInfo)
            Message.Publish(new SetEnemiesUiVisibility(msg.ShouldShow));
    }
}
