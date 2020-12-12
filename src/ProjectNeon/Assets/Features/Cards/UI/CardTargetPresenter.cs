using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardTargetPresenter : MonoBehaviour
{
    [SerializeField] private GameObject iconPanel;
    [SerializeField] private Image targetIcon;
    [SerializeField] private ColorReference alliesColor = new ColorReference(Color.green);
    [SerializeField] private ColorReference allColor = new ColorReference(Color.white);
    [SerializeField] private ColorReference opponentsColor = new ColorReference(Color.red);
    [SerializeField] private Sprite groupTargetSprite;
    [SerializeField] private Sprite singleTargetSprite;

    public void Set(CardTypeData c)
    {
        if (c.ActionSequences.None())
            Log.Error($"{c.Name} has no Action Sequences");
        var firstSeq = c.ActionSequences.First();
        targetIcon.sprite = firstSeq.Scope == Scope.One || firstSeq.Scope == Scope.OneExceptSelf
            ? singleTargetSprite
            : groupTargetSprite;

        if (firstSeq.Group == Group.Ally)
            targetIcon.color = alliesColor;
        else if (firstSeq.Group == Group.Opponent)
            targetIcon.color = opponentsColor;
        else if (firstSeq.Group == Group.All)
            targetIcon.color = allColor;
        
        iconPanel.SetActive(firstSeq.Group != Group.Self);
    }
}
