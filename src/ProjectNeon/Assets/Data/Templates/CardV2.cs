using UnityEngine;

public class CardV2 : ScriptableObject
{
    [SerializeField] private Sprite art;
    [SerializeField] private string description;
    [SerializeField] private StringVariable typeDescription;
    [SerializeField] private StringVariable onlyPlayableByClass;
    [SerializeField] private ResourceCost cost;
    [SerializeField] private CardV2 classSpecial;
    [SerializeField] private CardCommandAction[] commandActions;
    [SerializeField] private CardBattleActionV2[] battleActions;

    public CardCommandAction[] CommandActions => commandActions;
    public CardBattleActionV2[] BattleActions => battleActions;
}