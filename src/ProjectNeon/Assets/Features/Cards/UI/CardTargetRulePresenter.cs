using I2.Loc;
using UnityEngine;

public class CardTargetRulePresenter : MonoBehaviour, ILocalizeTerms
{
    [SerializeField] private Localize label;

    private const string TargetsOwner = "BattleUI/Targets Owner";
    private const string TargetsSingleEnemy = "BattleUI/Targets One Enemy";
    private const string TargetsAllEnemies = "BattleUI/Targets All Enemies";
    private const string TargetsAllEnemiesExcept = "BattleUI/Targets All Enemies Except";
    private const string TargetsAllEnemiesExceptOwner = "BattleUI/Targets All Enemies Except Owner";
    private const string TargetsSingleAlly = "BattleUI/Targets One Ally";
    private const string TargetsAllAllies = "BattleUI/Targets All Allies";
    private const string TargetsAllAlliesExcept = "BattleUI/Targets All Allies Except";
    private const string TargetsAllAlliesExceptOwner = "BattleUI/Targets All Allies Except Owner";
    private const string TargetsSingleNonOwnerAlly = "BattleUI/Targets One Non-Owner Ally";
    private const string TargetsEveryone = "BattleUI/Targets Everyone";
    private const string TargetsAnyone = "BattleUI/Targets Anyone";
    private const string TargetsAnyoneExceptOwner = "BattleUI/Targets Anyone Except Owner";
    private const string TargetsEveryoneExcept = "BattleUI/Targets Everyone Except";
    private const string TargetsEveryoneExceptOwner = "BattleUI/Targets Everyone Except Owner";
    private const string TargetsRandomEnemy = "BattleUI/Targets Random Enemy";
    private const string TargetsRandomAlly = "BattleUI/Targets Random Ally";
    private const string TargetsRandom = "BattleUI/Targets Random Character";
    
    public void Hide() => gameObject.SetActive(false);

    public void Show(CardActionSequence seq) => Show(seq.Scope, seq.Group);
    public void Show(Scope scope, Group group)
    {
        var term = string.Empty;
        if (group == Group.Self)
            term = TargetsOwner;
        else if (scope == Scope.One && group == Group.Opponent)
            term = TargetsSingleEnemy;
        else if (scope == Scope.All && group == Group.Opponent)
            term = TargetsAllEnemies;
        else if (scope == Scope.AllExcept && group == Group.Opponent)
            term = TargetsAllEnemiesExcept;
        else if (scope == Scope.AllExceptSelf && group == Group.Opponent)
            term = TargetsAllEnemiesExceptOwner;
        else if (scope == Scope.One && group == Group.Ally)
            term = TargetsSingleAlly;
        else if (scope == Scope.All && group == Group.Ally)
            term = TargetsAllAllies;
        else if (scope == Scope.AllExcept && group == Group.Ally)
            term = TargetsAllAlliesExcept;
        else if (scope == Scope.AllExceptSelf && group == Group.Ally)
            term = TargetsAllAlliesExceptOwner;
        else if (scope == Scope.OneExceptSelf && group == Group.Ally)
            term = TargetsSingleNonOwnerAlly;
        else if (scope == Scope.All && group == Group.All)
            term = TargetsEveryone;
        else if (scope == Scope.One && group == Group.All)
            term = TargetsAnyone;
        else if (scope == Scope.OneExceptSelf && group == Group.All)
            term = TargetsAnyoneExceptOwner;
        else if (scope == Scope.AllExcept && group == Group.All)
            term = TargetsEveryoneExcept;
        else if (scope == Scope.AllExceptSelf && group == Group.All)
            term = TargetsEveryoneExceptOwner;
        else if (scope == Scope.Random && group == Group.Ally)
            term = TargetsRandomAlly;
        else if (scope == Scope.Random && group == Group.Opponent)
            term = TargetsRandomEnemy;
        else if (scope == Scope.Random && group == Group.All)
            term = TargetsRandom;

        label.SetTerm(term);
        gameObject.SetActive(term.Length > 0);
    }

    public string[] GetLocalizeTerms()
        => new[]
        {
            TargetsOwner,
            TargetsSingleEnemy,
            TargetsAllEnemies,
            TargetsAllEnemiesExcept,
            TargetsAllEnemiesExceptOwner,
            TargetsSingleAlly,
            TargetsAllAllies,
            TargetsAllAlliesExcept,
            TargetsAllAlliesExceptOwner,
            TargetsSingleNonOwnerAlly,
            TargetsEveryone,
            TargetsAnyone,
            TargetsAnyoneExceptOwner,
            TargetsEveryoneExcept,
            TargetsEveryoneExceptOwner,
            TargetsRandom,
            TargetsRandomAlly,
            TargetsRandomEnemy
        };
}
