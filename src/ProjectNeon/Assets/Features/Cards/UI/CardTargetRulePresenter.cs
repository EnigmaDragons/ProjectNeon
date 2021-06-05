using TMPro;
using UnityEngine;

public class CardTargetRulePresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

    public void Hide() => gameObject.SetActive(false);

    public void Show(CardActionSequence seq) => Show(seq.Scope, seq.Group);
    public void Show(Scope scope, Group group)
    {
        gameObject.SetActive(false);
        var text = "";
        if (group == Group.Self)
            text = "Owner";
        else if (scope == Scope.One && group == Group.Opponent)
            text = "One Enemy";
        else if (scope == Scope.All && group == Group.Opponent)
            text = "All Enemies";
        else if (scope == Scope.AllExcept && group == Group.Opponent)
            text = "All Enemies Except";
        else if (scope == Scope.AllExceptSelf && group == Group.Opponent)
            text = "All Enemies Except Owner";
        else if (scope == Scope.One && group == Group.Ally)
            text = "One Ally";
        else if (scope == Scope.All && group == Group.Ally)
            text = "All Allies";
        else if (scope == Scope.AllExcept && group == Group.Ally)
            text = "All Allies Except";
        else if (scope == Scope.AllExceptSelf && group == Group.Ally)
            text = "All Allies Except Owner";
        else if (scope == Scope.OneExceptSelf && group == Group.Ally)
            text = "One Non-Owner Ally";
        else if (scope == Scope.All && group == Group.All)
            text = "Everyone";
        else if (scope == Scope.One && group == Group.All)
            text = "Anyone";
        else if (scope == Scope.OneExceptSelf && group == Group.All)
            text = "Anyone Except Owner";
        else if (scope == Scope.AllExcept && group == Group.All)
            text = "Everyone Except";
        else if (scope == Scope.AllExceptSelf && group == Group.All)
            text = "Everyone Except Owner";
        
        label.text = $"Targets {text}";
        if (text.Length > 0)
            gameObject.SetActive(true);
    }
}