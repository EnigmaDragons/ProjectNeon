using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleResolutions : OnMessage<ApplyBattleEffect, SpawnEnemy, DespawnEnemy, CardResolutionFinished, CardActionPrevented, WaitDuringResolution, ResolveReactionCards, ResolveReaction>
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
    private BattleReactions Reactions => state.Reactions;
    
    private bool _resolvingEffect;
    private float _playerDelayFactor = 0.2f;
    private bool _debugLog;

    private void DebugLog(string msg)
    {
        if (!_debugLog)
            return;
        
        DevLog.Info(msg);
    }
    
    private void ResolveNext()
    {
        PerformConsciousnessUpdate();
        if (reactionZone.Count > 0)
            reactionZone.Clear();
        
        if (Reactions.AnyReactionCards)
            StartCoroutine(ResolveNextReactionCard());
        else if (resolutionZone.HasMore)
            resolutionZone.BeginResolvingNext();
        else
            Message.Publish(new CardAndEffectsResolutionFinished());
    }

    private void PerformConsciousnessUpdate()
    {
        _unconsciousness.ProcessUnconsciousMembers(state)
            .ForEach(m => resolutionZone.ExpirePlayedCards(c => c.Member.Id == m.Id)); // Still needed?
        _unconsciousness.ProcessRevivedMembers(state);
    }

    public bool IsDoneResolving => state.BattleIsOver() || !Reactions.AnyReactionCards && resolutionZone.IsDone && !Reactions.AnyReactionEffects && !_resolvingEffect;

    protected override void Execute(ApplyBattleEffect msg)
    {
        var (result, maybeEffectResolved) = ApplyEffects(msg);
        var ctx = result.UpdatedContext;
        if (ctx.Selections.CardSelectionOptions?.Any() ?? false)
        {
            var action = ctx.Selections.OnCardSelected;
            ctx.Selections.OnCardSelected = card => { action(card); FinalizeBattleEffect(maybeEffectResolved); };
            Message.Publish(new PresentCardSelection { Selectons = ctx.Selections });
        }
        else
        {
            FinalizeBattleEffect(maybeEffectResolved);   
        }
    }

    private void FinalizeBattleEffect(Maybe<EffectResolved> maybeEffectResolved)
    {
        maybeEffectResolved.IfPresent(ApplyReactions);
        Message.Publish(new Finished<ApplyBattleEffect>());
    }

    private void ApplyReactions(EffectResolved e)
    {
        var reactions = state.Members
            .Select(x => x.Value)
            .SelectMany(v => v.State.GetReactions(e)).ToArray();

        DevLog.Write($"Reaction Timing {e.Timing}. Effects {reactions.Count(c => c.ReactionCard.IsMissing)}. " +
                     $"Cards: {reactions.Count(c => c.ReactionCard.IsPresent)}");

        Reactions.Enqueue(reactions);

        state.CleanupExpiredMemberStates();
        Message.Publish(e);
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
        if (msg.ToDecrement == TemporalStatType.Inhibit)
            BattleLog.Write($"{msg.Source.Name} was inhibited, so their action fizzled.");
        Message.Publish(new PlayRawBattleEffect("MissedText", Vector3.zero));
        msg.Source.State.Adjust(msg.ToDecrement, -1);
        Message.Publish(new Finished<CardActionPrevented>());
    }

    protected override void Execute(WaitDuringResolution msg)
    {
        Async.ExecuteAfterDelay(msg.Duration, () => Message.Publish(new Finished<WaitDuringResolution>()));
    }

    private (ApplyEffectResult, Maybe<EffectResolved>) ApplyEffects(ApplyBattleEffect msg)
    {
        DebugLog($"Apply Battle Effect {msg.Effect.EffectType} - Is Reaction: {msg.IsReaction}");
        // Core Execution
        var ctx = new EffectContext(msg.Source, msg.Target, msg.Card, msg.XPaidAmount, partyAdventureState, state.PlayerState, state.RewardState,
            state.Members, state.PlayerCardZones, msg.Preventions, new SelectionContext(), allCards.GetMap(), state.CreditsAtStartOfBattle, 
            state.Party.Credits, state.Enemies.ToDictionary(x => x.Member.Id, x => (EnemyType)x.Enemy), () => state.GetNextCardId(), 
            state.CurrentTurnCardPlays(), state.OwnerTints, state.OwnerBusts, msg.IsReaction, msg.Timing);
        var battleSnapshotBefore = state.GetSnapshot();
        var res = AllEffects.Apply(msg.Effect, ctx);
        
        // If Not Applicable Effect, Finish
        if (!res.WasApplied)
            return (res, Maybe<EffectResolved>.Missing());
        
        // Stealth Processing
        if (msg.Source.IsStealthed() && StealthBreakingEffectTypes.Contains(msg.Effect.EffectType))
        {
            msg.Source.State.BreakStealth();
            BattleLog.Write($"{msg.Source.Name} emerged from the shadows.");
        }

        // Effect Resolved Details
        var battleSnapshotAfter = state.GetSnapshot();
        var effectResolved = new EffectResolved(res.WasApplied, msg.IsFirstBattleEffect, msg.Effect, ctx.Source, ctx.Target, 
            battleSnapshotBefore, battleSnapshotAfter, ctx.IsReaction, ctx.Card, ctx.Preventions, ctx.Timing);
        return (res, effectResolved);
    }

    protected override void Execute(SpawnEnemy msg)
    {
        var details = enemies.Spawn(msg.Enemy.ForStage(state.Stage), msg.Offset);
        var member = details.Member;
        BattleLog.Write($"Spawned {member.Name}");
        Message.Publish(new MemberSpawned(member, details.Transform));
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
        if (Reactions.AnyReactionEffects)
        {
            _resolvingEffect = false;
            Reactions.ResolveNextInstantReaction(state.Members);
        }
        else
        {
            yield return new WaitForSeconds(DelaySeconds(resolutionZone.CurrentTeamType.OrDefault(TeamType.Enemies)));
            _resolvingEffect = false;
            resolutionZone.OnCardResolutionFinished();
            ResolveNext();
        }
    }

    protected override void Execute(ResolveReactionCards msg)
    {
        if (!_resolvingEffect && Reactions.AnyReactionCards)
            StartCoroutine(ResolveNextReactionCard());
    }

    protected override void Execute(ResolveReaction msg)
    {
        Log.Info("Resolve Reaction Message Received");
        StartCoroutine(ResolveNextReactionCard(msg.Reaction));
    }

    private IEnumerator ResolveNextReactionCard()
    {
        while (Reactions.AnyReactionCards)
        {
            if (_resolvingEffect)
                yield return new WaitUntil(() => !_resolvingEffect);
            else
            {
                var r = Reactions.DequeueNextReactionCard().WithPresentAndConsciousTargets(state.Members);
                yield return ResolveNextReactionCard(r);
            }
        }
        //Message.Publish(new Finished<ResolveReactionCards>());
    }
    
    private IEnumerator ResolveNextReactionCard(ProposedReaction r)
    {
        Log.Info(nameof(ResolveNextReactionCard));
        _resolvingEffect = true;
        if (reactionZone.Count > 0)
            reactionZone.Clear();
        
        var isReactionCard = r.ReactionCard.IsPresent;
        if (!isReactionCard)
        {
            Log.Error($"Should not be Queueing instant Effect Reactions. They should already be processed. " +
                      $"Reaction - {r.Source.Name} {r.Name} {r.ReactionSequence.CardActions.BattleEffects.First().EffectType}");
            StartCoroutine(FinishEffect());
            yield break;
        }
        
        if (!state.Members.ContainsKey(r.Source.Id) || !r.Target.Members.Any())
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
