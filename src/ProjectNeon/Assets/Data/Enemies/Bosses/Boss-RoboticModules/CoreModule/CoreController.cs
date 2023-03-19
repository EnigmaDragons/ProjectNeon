using System;
using UnityEngine;

public class CoreController : MemberInitable
{
    [SerializeField] private SpriteRenderer[] modules;
    [SerializeField] private CardType attachAegis;
    [SerializeField] private Sprite aegis;
    [SerializeField] private CardType attachDefense;
    [SerializeField] private Sprite defense;
    [SerializeField] private CardType attachDodge;
    [SerializeField] private Sprite dodge;
    [SerializeField] private CardType attachShield;
    [SerializeField] private Sprite shield;
    [SerializeField] private CardType attachStealth;
    [SerializeField] private Sprite stealth;
    [SerializeField] private GameObject[] toScale;
    [SerializeField] private float stage2Scale;

    private Member _member;
    private int _cardPlays;


    public override void Init(Member member)
    {
        _member = member;
        _cardPlays = (int)member.State[StatType.ExtraCardPlays];
    }

    private void OnEnable() => Message.Subscribe<CardResolutionFinished>(Execute, this);
    private void OnDisable() => Message.Unsubscribe(this);

    private void Execute(CardResolutionFinished card)
    {
        if (_member.State[StatType.ExtraCardPlays] == _cardPlays)
            return;
        toScale.ForEach(x => x.transform.localScale = new Vector3(stage2Scale, stage2Scale, 0));
        _cardPlays++;
        if (_cardPlays > modules.Length + 1)
            return;
        var gameObj = modules[_cardPlays - 2];
        if (card.CardTypeId == attachAegis.Id)
            gameObj.sprite = aegis;
        else if (card.CardTypeId == attachDefense.Id)
            gameObj.sprite = defense;
        else if (card.CardTypeId == attachDodge.Id)
            gameObj.sprite = dodge;
        else if (card.CardTypeId == attachShield.Id)
            gameObj.sprite = shield;
        else if (card.CardTypeId == attachStealth.Id)
            gameObj.sprite = stealth;
        gameObj.gameObject.SetActive(true);
    }
}