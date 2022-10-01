using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleUiVisibilityController : OnMessage<SetBattleUiElementVisibility>
{
    [SerializeField] private NamedGameObject[] uiElements;

    private Dictionary<string, GameObject> _uiElements;
    private Dictionary<string, HashSet<string>> _locks;

    private void Awake()
    {
        _uiElements = uiElements.SafeToDictionary(u => u.Name, u => u.Obj);
        _locks = new Dictionary<string, HashSet<string>>();
    }
    
    protected override void Execute(SetBattleUiElementVisibility msg)
    {
        HashSet<string> lockList;
        if (!_locks.TryGetValue(msg.UiElementName, out lockList))
            lockList = new HashSet<string>();
        if (msg.ShouldShow && lockList.Contains(msg.CallerId))
            lockList = lockList.Without(msg.CallerId).ToHashSet();
        if (!msg.ShouldShow && !lockList.Contains(msg.CallerId))
            lockList = lockList.With(msg.CallerId).ToHashSet();
        _locks[msg.UiElementName] = lockList;
        
        var shouldShow = !lockList.AnyNonAlloc();
        if (_uiElements.TryGetValue(msg.UiElementName, out var el))
            el.SetActive(shouldShow);
        if (msg.UiElementName == BattleUiElement.EnemyInfo || msg.UiElementName == BattleUiElement.EnemyTechPoints || msg.UiElementName == BattleUiElement.PrimaryStat)
            Message.Publish(new SetEnemiesUiVisibility(shouldShow, msg.UiElementName));
        if (msg.UiElementName == BattleUiElement.PrimaryStat || msg.UiElementName == BattleUiElement.PlayerResources || msg.UiElementName == BattleUiElement.PlayerShields)
            Message.Publish(new SetHeroesUiVisibility(shouldShow, msg.UiElementName));
    }
}
