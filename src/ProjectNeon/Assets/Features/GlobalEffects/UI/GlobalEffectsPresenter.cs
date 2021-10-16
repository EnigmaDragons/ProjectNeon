using System.Linq;
using UnityEngine;

public class GlobalEffectsPresenter : OnMessage<GlobalEffectsChanged>
{
    [SerializeField] private CurrentGlobalEffects effects;
    [SerializeField] private GameObject[] hideIfNoneActive;
    [SerializeField] private GameObject parent;
    [SerializeField] private GlobalEffectPresenter presenterProtoType;

    protected override void AfterEnable()
    {
        parent.DestroyAllChildren();
        Render(effects.Value);
    }

    protected override void Execute(GlobalEffectsChanged msg)
    {
        var current = msg.Effects;
        Render(current);
    }

    private void Render(GlobalEffect[] current)
    {
        var anyActive = current.Any();
        parent.DestroyAllChildren();
        hideIfNoneActive.ForEach(g => g.SetActive(anyActive));
        current.ForEach(e => Instantiate(presenterProtoType, parent.transform).Init(e));
    }
}
