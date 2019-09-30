using System.Linq;
using UnityEngine;

public class Card : ScriptableObject
{
    [SerializeField] private Sprite art;
    [SerializeField] private string description;
    [SerializeField] private string typeDescription;
    // @todo: #1:15min Limit this by Class. Create StringVariable for all classes to reduce fragility
    [SerializeField] private string onlyPlayableByHeroName;
    [SerializeField] private CardAction[] actions;

    public Sprite Art => art;
    public string Description => description;
    public string TypeDescription => typeDescription;
    public CardAction[] Actions => actions.ToArray();
    public Maybe<string> LimitedToHero => new Maybe<string>(onlyPlayableByHeroName);

}
