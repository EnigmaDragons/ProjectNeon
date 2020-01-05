﻿using System.Collections.Generic;
using UnityEngine;

public class PartyVisualizer : OnBattleEvent<CharacterAnimationRequested>
{
    [SerializeField] private BattleState battleState;
    [SerializeField] private GameObject hero1;
    [SerializeField] private GameObject hero2;
    [SerializeField] private GameObject hero3;
    [SerializeField] private GameEvent onPartySetupFinished;

    // @todo #125:15min Dynamically create Heroes from a Prototype, instead of fixed slots

    private readonly Dictionary<Hero, Animator> _animators = new Dictionary<Hero, Animator>();
    
    void Start()
    {
        var party = battleState.Party;
        var heroes = party.Heroes;
        SetupHero(hero1, heroes[0]);
        SetupHero(hero2, heroes[1]);
        SetupHero(hero3, heroes[2]);
        battleState.PartyArea.WithUiPositions(new[] { hero1.transform, hero2.transform, hero3.transform });
        onPartySetupFinished.Publish();
    }

    private void SetupHero(GameObject heroOrigin, Hero hero)
    {
        var hasBody = !hero.Body.name.Equals("BodyPlaceholder");
        if (hasBody)
        {
             var character = Instantiate(hero.Body, heroOrigin.transform.position, Quaternion.identity, heroOrigin.transform);
             _animators[hero] = character.GetComponentInChildren<Animator>();
        }
        else
        {
            heroOrigin.GetComponent<SpriteRenderer>().sprite = hero.Bust;
        }
    }

    protected override void Execute(CharacterAnimationRequested e)
    {
        if (!battleState.IsHero(e.MemberId)) return;
        
        var hero = battleState.GetHeroById(e.MemberId);
        var animator = _animators[hero];
        if (animator == null)
            Debug.LogWarning($"No Animator found for {battleState.GetHeroById(e.MemberId).name}");
        else
            animator.Play(e.Animation);
    }
}
