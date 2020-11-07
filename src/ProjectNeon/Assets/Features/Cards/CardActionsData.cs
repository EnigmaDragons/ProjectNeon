using UnityEngine;

[CreateAssetMenu(menuName = "Effect")]
public class CardActionsData : ScriptableObject
{
    public CardActionV2[] Actions = new CardActionV2[0];
        
    public CardActionsData Initialized(params CardActionV2[] actions)
    {
        Actions = actions;
        return this;
    }
}
