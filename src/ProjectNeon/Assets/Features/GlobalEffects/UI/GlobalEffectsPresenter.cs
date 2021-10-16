using System.Linq;
using UnityEngine;

public class GlobalEffectsPresenter : OnMessage<GlobalEffectsChanged>
{
    [SerializeField] private GameObject[] hideIfNoneActive;
    [SerializeField] private GameObject parent;
    [SerializeField] private GlobalEffectPresenter presenterProtoType;

    protected override void AfterEnable() => parent.DestroyAllChildren();
    
    protected override void Execute(GlobalEffectsChanged msg)
    {
        var effects = msg.Effects;
        var anyActive = effects.Any();
        parent.DestroyAllChildren();
        hideIfNoneActive.ForEach(g => g.SetActive(anyActive));
        effects.ForEach(e => Instantiate(presenterProtoType, parent.transform).Init(e));
    }
}
