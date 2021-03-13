using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Mainframe")]
public class MainframeAI : TurnAI
{
    private bool _hasbootedUp;
    private MainframePlan _currentPlan;
    private bool _isfirstCard;
    private int _stage;
    
    public override void InitForBattle()
    {
        _hasbootedUp = false;
        _currentPlan = MainframePlan.None;
        _isfirstCard = true;
        _stage = 1;
    }
    
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var maybeStatus = battleState.Members[memberId].State.StatusesOfType(StatusTag.OnDeath);
        _stage = maybeStatus.Any() ? maybeStatus.First().Amount.OrDefault(() => 0) == 2 ? 1 : 2 : 3;
        if (!_hasbootedUp)
        {
            _isfirstCard = false;
            _hasbootedUp = true;
            return new CardSelectionContext(memberId, battleState, strategy)
                .WithSelectedCardByNameIfPresent("Bootup")
                .WithSelectedTargetsPlayedCard();
        }
        else if (_isfirstCard)
        {
            _isfirstCard = false;
            if (_currentPlan == MainframePlan.Summon && battleState.Members[memberId].PrimaryResourceAmount() >= 2)
                return new CardSelectionContext(memberId, battleState, strategy)
                    .WithSelectedCardByNameIfPresent("Deploy Spiderling")
                    .WithSelectedTargetsPlayedCard();
            else if (_currentPlan == MainframePlan.Defend)
                return new CardSelectionContext(memberId, battleState, strategy)
                    .WithSelectedCardByNameIfPresent("Energy Discharge")
                    .WithSelectedTargetsPlayedCard();
            else if (_currentPlan == MainframePlan.Attack && _stage >= 2)
                return new CardSelectionContext(memberId, battleState, strategy)
                    .WithSelectedCardByNameIfPresent("Eliminate 2.0")
                    .WithSelectedTargetsPlayedCard(m => m.Members[0].Id == memberId);
            else if (_currentPlan == MainframePlan.Attack)
                return new CardSelectionContext(memberId, battleState, strategy)
                    .WithSelectedCardByNameIfPresent("Eliminate 1.0")
                    .WithSelectedTargetsPlayedCard(m => m.Members[0].Id == memberId);
            else
                return new CardSelectionContext(memberId, battleState, strategy)
                    .WithSelectedCardByNameIfPresent("Eliminate 1.0")
                    .WithSelectedTargetsPlayedCard();
        }
        else
        {
            _isfirstCard = true;
            if (_currentPlan != MainframePlan.Summon && battleState.Members[memberId].PrimaryResourceAmount() >= 4)
                _currentPlan = MainframePlan.Summon;
            else if (_currentPlan == MainframePlan.Attack)
                _currentPlan = MainframePlan.Defend;
            else if (_currentPlan == MainframePlan.Defend)
                _currentPlan = MainframePlan.Attack;
            else
                _currentPlan = Rng.Bool() ? MainframePlan.Attack : MainframePlan.Defend;
            if (_currentPlan == MainframePlan.Summon && _stage == 3)
                return new CardSelectionContext(memberId, battleState, strategy)
                    .WithSelectedCardByNameIfPresent("Deploy Seeker 3.0")
                    .WithSelectedTargetsPlayedCard();
            else if (_currentPlan == MainframePlan.Summon && _stage == 2)
                return new CardSelectionContext(memberId, battleState, strategy)
                    .WithSelectedCardByNameIfPresent("Deploy Seeker 2.0")
                    .WithSelectedTargetsPlayedCard();
            else if (_currentPlan == MainframePlan.Summon && _stage == 1)
                return new CardSelectionContext(memberId, battleState, strategy)
                    .WithSelectedCardByNameIfPresent("Deploy Seeker 1.0")
                    .WithSelectedTargetsPlayedCard();
            else if (_currentPlan == MainframePlan.Attack && _stage == 3)
                return new CardSelectionContext(memberId, battleState, strategy)
                    .WithSelectedCardByNameIfPresent("Track Target 2.0")
                    .WithSelectedTargetsPlayedCard();
            else if (_currentPlan == MainframePlan.Attack)
                return new CardSelectionContext(memberId, battleState, strategy)
                    .WithSelectedCardByNameIfPresent("Track Target 1.0")
                    .WithSelectedTargetsPlayedCard();
            else if (_currentPlan == MainframePlan.Defend && _stage == 3)
                return new CardSelectionContext(memberId, battleState, strategy)
                    .WithSelectedCardByNameIfPresent("Energy Buildup 3.0")
                    .WithSelectedTargetsPlayedCard();
            else if (_currentPlan == MainframePlan.Defend && _stage == 2)
                return new CardSelectionContext(memberId, battleState, strategy)
                    .WithSelectedCardByNameIfPresent("Energy Buildup 2.0")
                    .WithSelectedTargetsPlayedCard();
            else if (_currentPlan == MainframePlan.Defend)
                return new CardSelectionContext(memberId, battleState, strategy)
                    .WithSelectedCardByNameIfPresent("Energy Buildup 1.0")
                    .WithSelectedTargetsPlayedCard();
        }
        throw new Exception("Some pathway is not covered in this AI");
    }

    // This is a bogus anticipation. This AI needs to be rewritten.
    public override IPlayedCard Anticipate(int memberId, BattleState battleState, AIStrategy strategy) 
        => new CardSelectionContext(memberId, battleState, strategy)
            .WithSelectedTargetsPlayedCard();

    private enum MainframePlan
    {
        None,
        Summon,
        Defend,
        Attack
    }
}