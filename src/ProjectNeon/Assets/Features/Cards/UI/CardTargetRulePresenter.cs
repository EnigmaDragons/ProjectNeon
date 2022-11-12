using I2.Loc;
using UnityEngine;

public class CardTargetRulePresenter : MonoBehaviour
{
    [SerializeField] private Localize label;

    public void Hide() => gameObject.SetActive(false);

    public void Show(CardActionSequence seq) => Show(seq.Scope, seq.Group);
    public void Show(Scope scope, Group group)
    {
        var text = string.Empty;
        if (group == Group.Self)
            text = "Targets Owner";
        else if (scope == Scope.One && group == Group.Opponent)
            text = "Targets One Enemy";
        else if (scope == Scope.All && group == Group.Opponent)
            text = "Targets All Enemies";
        else if (scope == Scope.AllExcept && group == Group.Opponent)
            text = "Targets All Enemies Except";
        else if (scope == Scope.AllExceptSelf && group == Group.Opponent)
            text = "Targets All Enemies Except Owner";
        else if (scope == Scope.One && group == Group.Ally)
            text = "Targets One Ally";
        else if (scope == Scope.All && group == Group.Ally)
            text = "Targets All Allies";
        else if (scope == Scope.AllExcept && group == Group.Ally)
            text = "Targets All Allies Except";
        else if (scope == Scope.AllExceptSelf && group == Group.Ally)
            text = "Targets All Allies Except Owner";
        else if (scope == Scope.OneExceptSelf && group == Group.Ally)
            text = "Targets One Non-Owner Ally";
        else if (scope == Scope.All && group == Group.All)
            text = "Targets Everyone";
        else if (scope == Scope.One && group == Group.All)
            text = "Targets Anyone";
        else if (scope == Scope.OneExceptSelf && group == Group.All)
            text = "Targets Anyone Except Owner";
        else if (scope == Scope.AllExcept && group == Group.All)
            text = "Targets Everyone Except";
        else if (scope == Scope.AllExceptSelf && group == Group.All)
            text = "Targets Everyone Except Owner";

        label.SetTerm($"BattleUI/{text}");
        gameObject.SetActive(text.Length > 0);
    }
}
