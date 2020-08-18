using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Card")]
public class CardType : ScriptableObject, CardTypeData
{
    [PreviewSprite] [SerializeField] private Sprite art;
    [SerializeField] [TextArea(1, 12)] private string description;
    [SerializeField] private StringVariable typeDescription;
    [SerializeField] private CardTag[] tags;
    [SerializeField] private CharacterClass onlyPlayableByClass;
    [SerializeField] private ResourceCost cost;
    [SerializeField] private ResourceCost onPlayGain;
    [SerializeField] public CardActionSequence[] actionSequences = new CardActionSequence[0];

    public string Name => name.SkipThroughFirstDash().SkipThroughFirstUnderscore().WithSpaceBetweenWords();
    public ResourceCost Cost => cost;
    public ResourceCost Gain => onPlayGain;
    public Sprite Art => art;
    public string Description => description;
    public HashSet<CardTag> Tags => new HashSet<CardTag>(tags);
    public string TypeDescription => typeDescription.Value;
    public Maybe<CharacterClass> LimitedToClass => new Maybe<CharacterClass>(onlyPlayableByClass != null ? onlyPlayableByClass : null);
    public CardActionSequence[] ActionSequences => actionSequences == null ? new CardActionSequence[0] : actionSequences.ToArray();
    public CardActionsData[] Actions => ActionSequences.Select(a => a.CardActions).ToArray();

    public Card CreateInstance(int id, Member owner) => new Card(id, owner, this);
}
