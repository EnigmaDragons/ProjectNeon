using System.Linq;
using UnityEngine;

public class Card : ScriptableObject
{
    [SerializeField] private Sprite art;
    [SerializeField] private string description;
    [SerializeField] private string typeDescription;
    [SerializeField] private CardAction[] actions;
    [SerializeField] private Scope targetScope;
    [SerializeField] private Group targetGroup;

    public Sprite Art => art;
    public string Description => description;
    public string TypeDescription => typeDescription;
    public CardAction[] Actions => actions.ToArray();
    
    // @todo #1:15min Remove TargetScope and Group. Update PlayedCard to take Targets for each Card Action. Update AI to choose targets for each Action
    public Scope TargetScope => targetScope;
    public Group TargetGroup => targetGroup;
}
