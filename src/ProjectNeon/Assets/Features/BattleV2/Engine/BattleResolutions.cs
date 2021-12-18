using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class BattleResolutions : OnMessage<ApplyBattleEffect, SpawnEnemy, DespawnEnemy, CardResolutionFinished, CardActionPrevented, WaitDuringResolution>
{
    [SerializeField] private BattleState state;
    [SerializeField] private PartyAdventureState partyAdventureState;
    [SerializeField] private CardResolutionZone resolutionZone;
    [SerializeField] private CardPlayZone reactionZone;
    [SerializeField] private CardPlayZone currentResolvingCardZone;
    [SerializeField] private EnemyVisualizerV2 enemies;
    [SerializeField] private FloatReference delay = new FloatReference(1.8f);
    [SerializeField] private AllCards allCards;

    private readonly BattleUnconsciousnessChecker _unconsciousness = new BattleUnconsciousnessChecker();
    private readonly Queue<ProposedReaction> _instantReactions = new Queue<ProposedReaction>();
    private readonly Queue<ProposedReaction> _reactionCards = new Queue<ProposedReaction>();
    private bool _resolvingEffect;
    private float _playerDelayFactor = 0.2f;
    
    private void ResolveNext()
    {
        PerformConsciousnessUpdate();
        if (reactionZone.Count > 0)
            reactionZone.Clear();
        
        if (_reactionCards.Any())
            StartCoroutine(ResolveNextReactionCard());
        else if (resolutionZone.HasMore)
            resolutionZone.BeginResolvingNext();
        else
            Message.Publish(new CardAndEffectsResolutionFinished());
    }

    public void PerformConsciousnessUpdate()
    {
        _unconsciousness.ProcessUnconsciousMembers(state)
            .ForEach(m => resolutionZone.ExpirePlayedCards(c => c.Member.Id == m.Id)); // Still needed?
        _unconsciousness.ProcessRevivedMembers(state);
    }

    public bool IsDoneResolving => state.BattleIsOver() || _reactionCards.None() && resolutionZone.IsDone && _instantReactions.None() && !_resolvingEffect;

    private Maybe<(ApplyBattleEffect, BattleStateSnapshot, EffectContext)> _currentBattleEffectContext;

    protected override void Execute(ApplyBattleEffect msg)
    {
        var beforeState = state.GetSnapshot();
        var ctx = ApplyEffects(msg);
        _currentBattleEffectContext = new Maybe<(ApplyBattleEffect, BattleStateSnapshot, EffectContext)>((msg, state.GetSnapshot(), ctx));
        if (ctx.Selections.CardSelectionOptions?.Any() ?? false)
        {
            var action = ctx.Selections.OnCardSelected;
            ctx.Selections.OnCardSelected = card => { action(card); FinalizeBattleEffect(); };
            Message.Publish(new PresentCardSelection { Selectons = ctx.Selections });
        }
        else
        {
            FinalizeBattleEffect();   
        }
    }

    private void FinalizeBattleEffect()
    {
        var (msg, battleSnapshotBefore, ctx) = _currentBattleEffectContext.Value;
        var battleSnapshotAfter = state.GetSnapshot();
        
        var effectResolved = new EffectResolved(msg.IsFirstBattleEffect, msg.Effect, ctx.Source, ctx.Target, battleSnapshotBefore, battleSnapshotAfter, ctx.IsReaction, ctx.Card, ctx.Preventions);
        var reactions = state.Members
            .Select(x => x.Value)
            .SelectMany(v => v.State.GetReactions(effectResolved)).ToList();

        var immediateReactions = reactions.Where(r => r.ReactionCard.IsMissing);
        immediateReactions.ForEach(r => _instantReactions.Enqueue(r));
        
        var reactionCards = reactions.Where(r => r.ReactionCard.IsPresent);
        reactionCards.ForEach(r => _reactionCards.Enqueue(r));
        
        state.CleanupExpiredMemberStates();
        
        _currentBattleEffectContext = Maybe<(ApplyBattleEffect, BattleStateSnapshot, EffectContext)>.Missing();

        Message.Publish(new Finished<ApplyBattleEffect>());
    }

    private static readonly HashSet<EffectType> StealthBreakingEffectTypes = new HashSet<EffectType>(new []
    {
        EffectType.AttackFormula, 
        EffectType.Kill, 
        EffectType.DamageOverTimeFormula,
        EffectType.MagicAttackFormula, 
    });
    
    protected override void Execute(CardActionPrevented msg)
    {
        if (msg.ToDecrement == TemporalStatType.Blind)
            BattleLog.Write($"{msg.Source.Name} was blinded, so their attack missed.");
        Message.Publish(new PlayRawBattleEffect("MissedText", Vector3.zero));
        msg.Source.State.Adjust(msg.ToDecrement, -1);
        Message.Publish(new Finished<CardActionPrevented>());
    }

    protected override void Execute(WaitDuringResolution msg)
    {
        Async.ExecuteAfterDelay(msg.Duration, () => Message.Publish(new Finished<WaitDuringResolution>()));
    }

    private EffectContext ApplyEffects(ApplyBattleEffect msg)
    {
        // Core Execution
        var ctx = new EffectContext(msg.Source, msg.Target, msg.Card, msg.XPaidAmount, partyAdventureState, state.PlayerState, state.RewardState,
            state.Members, state.PlayerCardZones, msg.Preventions, new SelectionContext(), allCards.GetMap(), state.CreditsAtStartOfBattle, 
            state.Party.Credits, state.Enemies.ToDictionary(x => x.Member.Id, x => (EnemyType)x.Enemy), () => state.GetNextCardId(), 
            state.CurrentTurnCardPlays(), state.OwnerTints, state.OwnerBusts, msg.IsReaction);
        var effectResult = AllEffects.Apply(msg.Effect, ctx);

        // Stealth Processing
        if (effectResult.WasApplied && msg.Source.IsStealthed() && StealthBreakingEffectTypes.Contains(msg.Effect.EffectType))
        {
            msg.Source.State.BreakStealth();
            BattleLog.Write($"{msg.Source.Name} emerged from the shadows.");
        }

        return effectResult.UpdatedContext;
    }

    protected override void Execute(SpawnEnemy msg)
    {
        var member = enemies.Spawn(msg.Enemy.ForStage(state.Stage), msg.Offset);
        BattleLog.Write($"Spawned {member.Name}");
        Message.Publish(new MemberSpawned(member, state.GetTransform(member.Id)));
        Message.Publish(new Finished<SpawnEnemy>());
    }
    
    protected override void Execute(DespawnEnemy msg)
    {
        var pos = state.GetMaybeTransform(msg.Member.Id).Map(t => t.position).OrDefault(Vector3.zero);
        enemies.Despawn(msg.Member.State);
        BattleLog.Write($"Despawned {msg.Member.Name}");
        Message.Publish(new MemberDespawned(msg.Member, pos));
        Message.Publish(new Finished<DespawnEnemy>());
    }

    protected override void Execute(CardResolutionFinished msg) => StartCoroutine(FinishEffect());

    private IEnumerator FinishEffect()
    {
        state.CleanupExpiredMemberStates();
        resolutionZone.ExpirePlayedCards(c => !state.Members.ContainsKey(c.MemberId()));
        if (_instantReactions.Any())
        {
            _resolvingEffect = false;
            BeginResolvingNextInstantReaction();
        }
        else
        {
            yield return new WaitForSeconds(DelaySeconds(resolutionZone.CurrentTeamType.OrDefault(TeamType.Enemies)));
            _resolvingEffect = false;
            resolutionZone.OnCardResolutionFinished();
            ResolveNext();
        }
    }

    private void BeginResolvingNextInstantReaction()
    {
        var r = _instantReactions.Dequeue();
        r.ReactionSequence.Perform(r.Name, r.Source, r.Target, ResourceQuantity.None);
    }
    
    private IEnumerator ResolveNextReactionCard()
    {
        _resolvingEffect = true;
        var r = _reactionCards.Dequeue();
        var isReactionCard = r.ReactionCard.IsPresent;
        if (!isReactionCard)
        {
            _resolvingEffect = false;
            Log.Error("Should not be Queueing instant Effect Reactions. They should already be processed.");
            yield break;
        }

        if (!state.Members.ContainsKey(r.Source.Id))
        {
            StartCoroutine(FinishEffect());
            yield break;
        }

        var reactionCard = r.ReactionCard.Value;
        if (reactionCard.IsPlayableBy(r.Source, state.Party, 1))
        {
            BattleLog.Write($"{r.Source.Name} has reacted with {reactionCard.Name}");
            Message.Publish(new DisplayCharacterWordRequested(r.Source, CharacterReactionType.ReactionCardPlayed));
            var card = new Card(state.GetNextCardId(), r.Source, reactionCard);
            if (r.Source.TeamType == TeamType.Party)
                card = new Card(state.GetNextCardId(), r.Source, reactionCard, state.GetHeroById(r.Source.Id).Tint, state.GetHeroById(r.Source.Id).Bust);
            reactionZone.PutOnBottom(card);
            currentResolvingCardZone.Set(card);
            yield return new WaitForSeconds(DelaySeconds(card.Owner.TeamType));
            
            var resourceCalculations = r.Source.CalculateResources(reactionCard);
            var playedCard = new PlayedCardV2(r.Source, new[] {r.Target}, card, true, resourceCalculations);
            r.Source.Apply(s => s.Lose(resourceCalculations.PaidQuantity, state.Party));
            resolutionZone.StartResolvingOneCard(playedCard, () => reactionCard.ActionSequence.Perform(r.Name, r.Source, r.Target, resourceCalculations.XAmountQuantity));
        }
        else
        {
            BattleLog.Write($"{r.Source.Name} could not afford reaction card {reactionCard.Name}");
            this.ExecuteAfterDelay(() => StartCoroutine(FinishEffect()), 0.1f);
        }
    }

    private float DelaySeconds(TeamType team) 
        => delay.Value * (team == TeamType.Party ? _playerDelayFactor : 1f);
}
