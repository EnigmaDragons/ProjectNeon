using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "GameState/BattleState")]
public class BattleState : ScriptableObject
{
    [SerializeField] private CardPlayZones cardPlayZones;
    [SerializeField] private CardResolutionZone resolutionZone;
    [SerializeField] private PartyArea partyArea;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private EnemyArea enemies;
    [SerializeField] private CurrentAdventure adventure;
    [SerializeField] private bool needsCleanup;
    [SerializeField] private bool isEliteBattle;
    [SerializeField] private BattleV2Phase phase;
    [SerializeField] private CurrentAdventureProgress adventureProgress;
    [SerializeField] private CurrentGameMap3 map;
    [SerializeField] private AllCards allCards;
    [SerializeField] private BattleRewardState rewards;

    [Header("Next Encounter")] [SerializeField]
    private GameObject nextBattlegroundPrototype;

    [SerializeField] private EnemyInstance[] nextEnemies;
    [SerializeField] private bool nextIsEliteBattle;

    [Header("ReadOnly")] [SerializeField, ReadOnly]
    private List<string> memberNames;

    [SerializeField, ReadOnly] private int turnNumber;
    [SerializeField, ReadOnly] private PlayerState playerState = new PlayerState();

    private List<List<PlayedCardSnapshot>> _playedCardHistory = new List<List<PlayedCardSnapshot>>();
    private readonly CurrentBattleStats _currentBattleStats = new CurrentBattleStats();
    private int _numPlayerDiscardsUsedThisTurn = 0;
    private bool _runStatsWritten = false;
    private EffectScopedData _effectScopedData = new EffectScopedData();
    public EffectScopedData EffectScopedData => _effectScopedData;

    public void ResetEffectScopedData()
        => _effectScopedData = new EffectScopedData();

    public int CreditsAtStartOfBattle { get; private set; }
    public bool IsSelectingTargets = false;
    public BattleV2Phase Phase => phase;
    public int TurnNumber => turnNumber;
    public int NumberOfRecyclesRemainingThisTurn => PlayerState.NumberOfRecyclesRemainingThisTurn;

    private int CurrentTurnPartyNonBonusStandardCardPlays => CurrentTurnCardPlays()
        .Count(x => x.Member.TeamType == TeamType.Party && !x.WasTransient && x.CardSpeed == CardSpeed.Standard);

    public int NumberOfCardPlaysRemainingThisTurn => playerState.CurrentStats.CardPlays() -
                                                     CurrentTurnPartyNonBonusStandardCardPlays -
                                                     _numPlayerDiscardsUsedThisTurn;

    public PlayedCardSnapshot[] CurrentTurnCardPlays() => _playedCardHistory.Count > 0
        ? _playedCardHistory.Last().ToArray()
        : Array.Empty<PlayedCardSnapshot>();

    public CurrentBattleStats Stats => _currentBattleStats;
    public BattleRewardState RewardState => rewards;
    public int RewardCredits => rewards.RewardCredits;
    public int RewardXp => rewards.RewardXp;
    public int PredictedTotalRewardXp => rewards.RewardXp + (_heroesById.First().Value.Levels.XpRequiredForNextLevel * adventureProgress.AdventureProgress.BonusXpLevelFactor).CeilingInt();
    public int RewardClinicVouchers => adventure.Adventure.BattleRewardClinicVouchers;
    public CardType[] RewardCards => rewards.RewardCards;
    public StaticEquipment[] RewardEquipments => rewards.RewardEquipments;

    public bool HasCustomEnemyEncounter => nextEnemies != null && nextEnemies.Length > 0;
    public EnemyInstance[] NextEncounterEnemies => nextEnemies.ToArray();
    public int Stage => adventureProgress.AdventureProgress.CurrentChapterNumber;
    public bool NeedsCleanup => needsCleanup;
    public bool IsEliteBattle => isEliteBattle;
    public float CreditPerPowerLevelRewardFactor => adventure.Adventure?.RewardCreditsPerPowerLevel ?? 0;
    public float XpPerPowerLevelRewardFactor => adventure.Adventure?.XpPerPowerLevel ?? 0;
    public CardPlayZones PlayerCardZones => cardPlayZones;
    public PartyAdventureState Party => party;
    public PartyArea PartyArea => partyArea;
    public EnemyArea EnemyArea => enemies;
    public GameObject Battlefield => nextBattlegroundPrototype;
    public IDictionary<int, Member> Members => _membersById;
    public Member[] MembersWithoutIds => Members.Values.ToArray();
    public Member[] Heroes => Members.Values.Where(m => m.TeamType == TeamType.Party).ToArray();
    public Member[] EnemyMembers => Members.Values.Where(m => m.TeamType == TeamType.Enemies).ToArray();

    public Member[] ConsciousEnemyMembers =>
        Members.Values.Where(m => m.TeamType == TeamType.Enemies && m.IsConscious()).ToArray();

    public (Member Member, EnemyInstance Enemy)[] Enemies => EnemyMembers.Select(m => (m, _enemiesById[m.Id])).ToArray();

    public PlayerState PlayerState => playerState;
    public AllCards AllCards => allCards;
    public BattleReactions Reactions { get; private set; }

    public int BattleRngSeed => adventureProgress.HasActiveAdventure
        ? adventureProgress.AdventureProgress.RngSeed
        : Rng.NewSeed();

    private Dictionary<int, EnemyInstance> _enemiesById = new Dictionary<int, EnemyInstance>();
    private Dictionary<int, Hero> _heroesById = new Dictionary<int, Hero>();
    private Dictionary<int, Member> _membersById = new Dictionary<int, Member>();
    private Dictionary<int, Transform> _uiTransformsById = new Dictionary<int, Transform>();
    private Dictionary<int, Member> _unconsciousMembers = new Dictionary<int, Member>();
    private EnemyInstance[] _battleStartingEnemies;
    private BattleAttritionTracker _tracker;

    // Tutorial State. Pretty hacky.
    public bool IsStoryEventCombat { get; private set; }
    public bool DontShuffleNextBattle { get; private set; }
    public bool IsTutorialCombat { get; private set; }
    public Maybe<int> OverrideStartingPlayerCards { get; private set; } = Maybe<int>.Missing();
    public bool ShowSwapCardForBasic { get; private set; } = true;
    public bool ShowCycleOrDiscard { get; private set; } = true;
    public bool AllowRightClickOnCard { get; private set; } = true;
    public bool AllowMiddleClickOnCard { get; private set; } = true;
    public bool BasicSuperFocusEnabled { get; private set; } = false;
    public bool IsSingleTutorialBattle { get; set; } = false;
    public CardType[] OverrideDeck { get; private set; }

    public MemberMaterialType MaterialTypeOf(int memberId)
        => _membersById.ValueOrMaybe(memberId).Select(m => m.MaterialType, MemberMaterialType.Unknown);

    // Setup

    public void SetNextBattleground(GameObject prototype)
    {
        nextBattlegroundPrototype = prototype;
        #if UNITY_EDITOR
        DevLog.Write($"Next Battlefield is {prototype.name}");
        #endif
    }

    public void SetNextEncounter(IEnumerable<EnemyInstance> e, bool isElite = false, bool isStoryEventCombat = false,
        bool isTutorialCombat = false, CardType[] overrideDeck = null)
    {
        nextEnemies = e.ToArray();
        nextIsEliteBattle = isElite;
        IsStoryEventCombat = isStoryEventCombat;
        DontShuffleNextBattle = isTutorialCombat;
        IsTutorialCombat = isTutorialCombat;
        OverrideDeck = overrideDeck;
        OverrideStartingPlayerCards = Maybe<int>.Missing();
        ShowSwapCardForBasic = true;
        ShowCycleOrDiscard = true;
        AllowRightClickOnCard = true;
        AllowMiddleClickOnCard = true;
        BasicSuperFocusEnabled = false;
        #if UNITY_EDITOR
        DevLog.Write($"Next Encounter has {string.Join(", ", nextEnemies.Select(x => x.NameTerm.ToEnglish()))}");
        #endif
    }

    public void SetNextBattleStartingCardCount(int cardCount)
    {
        OverrideStartingPlayerCards = cardCount;
        #if UNITY_EDITOR
        DevLog.Write($"Next Encounter has {cardCount} Starting Cards");
        #endif
    }

    public void SetAllowSwapToBasic(bool shouldAllow)
    {
        ShowSwapCardForBasic = shouldAllow;
        AllowRightClickOnCard = shouldAllow;
        #if UNITY_EDITOR
        DevLog.Write($"Next Encounter has Allow Swap to Basic {shouldAllow}");
        #endif
    }
    
    public void SetAllowCycleOrDiscard(bool shouldAllow)
    {
        ShowCycleOrDiscard = shouldAllow;
        AllowMiddleClickOnCard = shouldAllow;
        #if UNITY_EDITOR
        DevLog.Write($"Next Encounter has Allow Cycle or Discard {shouldAllow}");
        #endif
    }

    public void SetBasicSuperFocusEnabled(bool enabled)
    {
        if (enabled == BasicSuperFocusEnabled)
            return;
        if (enabled)
            UpdateState(SetBasicSuperFocusIsEnabledAlloc);
        else
            UpdateState(SetBasicSuperFocusIsNotEnabledAlloc);
    }
    public void SetBasicSuperFocusIsEnabledAlloc() => BasicSuperFocusEnabled = true;
    public void SetBasicSuperFocusIsNotEnabledAlloc() => BasicSuperFocusEnabled = false;

    private void LogEncounterInfo(bool isElite, int targetPower, int actualPower)
    {
        var factor = actualPower / (float) targetPower;
        DevLog.Write($"{(isElite ? "Elite " : string.Empty)}Encounter: Target Power Level {targetPower}. Actual: {actualPower}. Factor: {factor:F2}");
    }

    public void SetupEnemyEncounter()
    {
        LogEncounterInfo();

        _battleStartingEnemies = nextEnemies.ToArray();
        EnemyArea.Initialized(nextEnemies);
        isEliteBattle = nextIsEliteBattle;
        nextEnemies = new EnemyInstance[0];
        nextIsEliteBattle = false;
    }

    private void LogEncounterInfo()
    {
        try
        {
            if (adventureProgress.HasActiveAdventure)
                if (isEliteBattle)
                    LogEncounterInfo(true, adventureProgress.AdventureProgress.CurrentElitePowerLevel,
                        nextEnemies.Sum(e => e.PowerLevel));
                else
                    LogEncounterInfo(false, adventureProgress.AdventureProgress.CurrentPowerLevel,
                        nextEnemies.Sum(e => e.PowerLevel));
        }
        catch (Exception e)
        {
            Log.Info("Unable to Log Adventure Progress Info for Battle");
        }
    }

    public void Init()
    {
        SetPhase(BattleV2Phase.NotBegun);
        NextCardId.Reset();
        _tracker = BattleAttritionTracker.Start(party);
        Reactions = new BattleReactions(new Queue<ProposedReaction>(), new Queue<ProposedReaction>());
        IsSelectingTargets = false;
        turnNumber = 0;
        _currentBattleStats.Clear();
        _runStatsWritten = false;
        ResetEffectScopedData();
    }

    public void SetPhase(BattleV2Phase p)
    {
        UpdateState(() => phase = p);
        Message.Publish(new BattlePhaseChanged(p));
    }
    public int GetNextCardId() => NextCardId.Get();

    private int EnemyStartingIndex => 4;
    private int _nextEnemyId = 0;
    public int GetNextEnemyId() => _nextEnemyId++;
    public List<Tuple<int, Member>> FinishSetup()
    {
        var id = 0;
        _uiTransformsById = new Dictionary<int, Transform>();

        var heroes = Party.Heroes;
        _heroesById = new Dictionary<int, Hero>();
        for (var i = 0; i < Party.BaseHeroes.Length; i++)
        {
            id++;
            var hero = heroes[i];
            var heroTransform = partyArea.UiPositions[i];
            _heroesById[id] = hero;
            _uiTransformsById[id] = heroTransform;
            heroTransform.GetComponentInChildren<ActiveMemberIndicator>()?.Init(id, true);
            heroTransform.GetComponentInChildren<CharacterCreatorAnimationController>()?.Init(id, hero.Character.Animations, TeamType.Party);
            heroTransform.GetComponentInChildren<DeathPresenter>()?.Init(id);
            if (hero.Character.AnimationSounds != null)
            {
                var sound = heroTransform.GetComponentInChildren<CharacterAnimationSoundPlayer>();
                if (sound == null)
                {
                    Log.Warn($"{hero.NameTerm.ToEnglish()} is missing CharacterAnimationSoundPlayer");
                    sound = heroTransform.gameObject.AddComponent<CharacterAnimationSoundPlayer>();
                }
                if (sound != null)
                    sound.Init(id, hero.Character.AnimationSounds, heroTransform);
                
            }
        }

        id = EnemyStartingIndex - 1;
        _enemiesById = new Dictionary<int, EnemyInstance>();
        var result = new List<Tuple<int, Member>>();
        for (var i = 0; i < enemies.Enemies.Count; i++)
        {
            id++;
            var enemy = enemies.Enemies[i];
            var enemyTransform = enemies.EnemyUiPositions[i];
            _enemiesById[id] = enemy;
            _uiTransformsById[id] = enemyTransform;
            _uiTransformsById[id].GetComponent<ActiveMemberIndicator>()?.Init(id, false);
            _uiTransformsById[id].GetComponentInChildren<CharacterCreatorAnimationController>()?.Init(id, _enemiesById[id].Animations, TeamType.Enemies);
            _uiTransformsById[id].GetComponentInChildren<DeathPresenter>()?.Init(id);
            if(enemy.AnimationSounds != null)
                _uiTransformsById[id].GetComponentInChildren<CharacterAnimationSoundPlayer>()?.Init(id, enemy.AnimationSounds, enemyTransform);
            result.Add(new Tuple<int, Member>(i, enemy.AsMember(id)));
        }
        _nextEnemyId = id + 1;

        playerState = new PlayerState(adventure?.Adventure?.BaseNumberOfCardCycles ?? 0);
        
        _membersById = new Dictionary<int, Member>();
        foreach (var m in _heroesById)
        {
            var member = m.Value.AsMember(m.Key);
            _membersById[member.Id] = member;
        }
        foreach (var enemy in result)
            _membersById[enemy.Item1] = enemy.Item2;

        foreach (var h in _heroesById)
            h.Value.InitState(_membersById[h.Key], this);
        foreach (var e in _enemiesById)
            e.Value.SetupMemberState(_membersById[e.Key], this);

        PlayerState.NumberOfCyclesUsedThisTurn = 0;
        _numPlayerDiscardsUsedThisTurn = 0;
        rewards.Init();
        needsCleanup = true;
        _unconsciousMembers = new Dictionary<int, Member>();
        _playedCardHistory = new List<List<PlayedCardSnapshot>> { new List<PlayedCardSnapshot>() };
        turnNumber = 1;
        CreditsAtStartOfBattle = party.Credits;
        ExceptionSuppressor.LogAndContinue(() => party.ApplyBlessings(this), "Applying Party Blessings");

        #if UNITY_EDITOR
        DevLog.Write("Finished Battle State Init");
        #endif
        return result;
    }

    public void ApplyAllGlobalStartOfBattleEffects()
    {
        if (adventureProgress.HasActiveAdventure)
            adventureProgress.AdventureProgress.GlobalEffects.StartOfBattleEffects.ForEach(e =>
            {
                if (_membersById.None())
                    return;
                
                var heroLeader = _membersById.First().Value;
                
                var possibleTargets = this.GetPossibleConsciousTargets(heroLeader, e.Group, e.Scope);
                if (possibleTargets.None())
                    return;
                
                var target = possibleTargets.First();
                if (target.Members.None())
                    return;
                
                var src = target.Members.First();
                var ctx = EffectContext.ForEffectFromBattleState(this, src, target, ReactionTimingWindow.FirstCause);
                AllEffects.Apply(e.EffectData, ctx);
            });
    }

    public void CleanupIfNeeded()
    {
        if (!NeedsCleanup) return;
        
        EnemyArea.Clear();
        needsCleanup = false;
        #if UNITY_EDITOR
        DevLog.Write("Finished Battle State Cleanup");
        #endif
    }

    // During Battle State Tracking
    public void StartTurn() => UpdateState(() => PlayerState.OnTurnStart());
    public void RecordPlayedCard(IPlayedCard card) => UpdateState(() =>
    {
        _playedCardHistory.Last().Add(new PlayedCardSnapshot(card));
        if (card.Member.TeamType == TeamType.Party)
            Stats.CardsPlayed++;
    });
    public void RecordCardDiscarded() => UpdateState(() => _numPlayerDiscardsUsedThisTurn++);
    public void CleanupExpiredMemberStates() => UpdateState(CleanupExpiredMemberStatesAlloc);
    private void CleanupExpiredMemberStatesAlloc()
    {
        foreach (var m in _membersById)
            m.Value.State.CleanExpiredStates();
    }

    public void RecordSingleCardDamageDealt(BattleStateSnapshot before)
    {
        var totalBefore = before.Members.Where(m => m.Value.TeamType == TeamType.Enemies).Sum(m => m.Value.State.HpAndShieldWithOverkill);
        var totalAfter = Enemies.Sum(e => e.Member.HpAndShieldWithOverkill());
        RecordSingleCardDamageDealt(totalBefore - totalAfter);
    }

    private void RecordSingleCardDamageDealt(int amount)
    {
        if (TurnNumber > 3)
            return;

        Stats.HighestPreTurn4CardDamage = Math.Max(amount, Stats.HighestPreTurn4CardDamage);
        if (Stats.HighestPreTurn4CardDamage > PermanentStats.Data.HighestPreTurn4SingleCardDamage)
            PermanentStats.Mutate(s => s.HighestPreTurn4SingleCardDamage = Stats.HighestPreTurn4CardDamage);
    }

    public Member[] GetAllNewlyUnconsciousMembers()
    {
        var newlyUnconscious = Members
            .Where(m => !_unconsciousMembers.ContainsKey(m.Key))
            .Select(x => x.Value)
            .Where(m => !m.State.IsConscious)
            .ToArray();
        newlyUnconscious.ForEach(r => _unconsciousMembers[r.Id] = r);
        return newlyUnconscious;
    }

    public Member[] GetAllNewlyRevivedMembers()
    {
        var newlyConscious = Members
            .Where(m => _unconsciousMembers.ContainsKey(m.Key))
            .Select(x => x.Value)
            .Where(m => m.State.IsConscious)
            .ToArray();
        newlyConscious.ForEach(r => _unconsciousMembers.Remove(r.Id));
        return newlyConscious;
    }

    public void AdvanceTurn() =>
        UpdateState(() =>
        {
            _playedCardHistory.Add(new List<PlayedCardSnapshot>());
            _numPlayerDiscardsUsedThisTurn = 0;
            PlayerState.OnTurnEnd();
            turnNumber++;
        });

    public void UseRecycle() => UpdateState(() => PlayerState.NumberOfCyclesUsedThisTurn++);
    public void AddEnemyDefeatedRewards(int memberId)
    {
        var enemy = GetEnemyById(memberId);
        AddRewardCredits(enemy.GetRewardCredits(CreditPerPowerLevelRewardFactor));
        AddRewardXp(enemy.GetRewardXp(XpPerPowerLevelRewardFactor));
    }
    public void AddRewardCredits(int amount) => UpdateState(() => rewards.AddRewardCredits(amount));
    public void AddRewardXp(int xp) => UpdateState(() => rewards.AddRewardXp(xp)); 
    public void SetRewardCards(params CardType[] cards) => UpdateState(() => rewards.SetRewardCards(cards));
    public void SetRewardEquipment(params StaticEquipment[] equipments) => UpdateState(() => rewards.SetRewardEquipment(equipments));

    public void AddEnemy(EnemyInstance e, GameObject gameObject, Member member) 
        => UpdateState(() =>
        {
            EnemyArea.Add(e, gameObject.transform);
            _enemiesById[member.Id] = e;
            _membersById[member.Id] = member;
            _uiTransformsById[member.Id] = gameObject.transform;
            _uiTransformsById[member.Id].GetComponent<ActiveMemberIndicator>()?.Init(member.Id, false);
            _uiTransformsById[member.Id].GetComponentInChildren<CharacterCreatorAnimationController>()?.Init(member.Id, _enemiesById[member.Id].Animations, TeamType.Enemies);
            _uiTransformsById[member.Id].GetComponentInChildren<DeathPresenter>()?.Init(member.Id);
            if(e.AnimationSounds != null)
                _uiTransformsById[member.Id].GetComponentInChildren<CharacterAnimationSoundPlayer>()?.Init(member.Id, e.AnimationSounds, gameObject.transform);
            e.SetupMemberState(member, this);
        });

    public void RemoveEnemy(MemberState member)
        => UpdateState(() =>
        {
            if (_enemiesById.TryGetValue(member.MemberId, out var enemy)) 
                EnemyArea.Remove(enemy);
            _enemiesById.Remove(member.MemberId);
            _membersById.Remove(member.MemberId);
            _uiTransformsById.Remove(member.MemberId);
            
        });

    // Battle Wrapup
    public void Wrapup()
    {
        EnemyArea.Clear();
        if (IsSingleTutorialBattle || !adventureProgress.HasActiveAdventure)
            return;
        
        EncounterIdTrackingState.MarkEncounterFinished();
        RecordPartyAdventureHp();
        var battleAttritionReport = _tracker.Finalize(party);
        GrantRewardCredits();
        GrantRewardCards();
        GrantRewardEquipment();
        GrantRewardClinicVouchers();
        var xpGranted = GrantXp();
        var battleSummaryReport = new BattleSummaryReport
        {
            enemies = _battleStartingEnemies.Select(e => e.NameTerm.ToEnglish()).ToArray(),
            fightTier = (isEliteBattle ? EnemyTier.Elite : EnemyTier.Normal).ToString(),
            totalEnemyPowerLevel = _battleStartingEnemies.Sum(e => e.PowerLevel),
            attritionCreditsChange = battleAttritionReport.TotalCreditsChange,
            attritionHpChange = battleAttritionReport.TotalHpChange,
            attritionInjuriesChange = battleAttritionReport.TotalInjuriesChange,
            rewardXp = xpGranted,
            rewardCredits = RewardCredits,
            rewardCards = RewardCards.Select(c => c.Name).ToArray(),
            rewardGear = RewardEquipments.Select(e => e.GetMetricNameOrDescription()).ToArray() 
        };
        Log.Info($"Battle Xp {xpGranted} - Creds {RewardCredits}");
        AllMetrics.PublishBattleSummary(battleSummaryReport);
        AccumulateRunStats();
    }

    public void AccumulateRunStats()
    {
        if (_runStatsWritten)
            return;

        _runStatsWritten = true;
        CurrentGameData.WriteIfInitialized(d =>
        {
            d.Stats.TotalTurnsPlayed += TurnNumber;
            d.Stats.TotalCardsPlayed += Stats.CardsPlayed;
            d.Stats.TotalEnemiesKilled += Enemies.Count(e => e.Member.IsUnconscious());
            d.Stats.TotalCardsPlayed += Stats.CardsPlayed;
            d.Stats.TotalDamageDealt += Stats.DamageDealt;
            d.Stats.TotalDamageReceived += Stats.DamageReceived;
            d.Stats.TotalHpDamageReceived += Stats.HpDamageReceived;
            d.Stats.TotalHealingReceived += Stats.HealingReceived;
            d.Stats.HighestPreTurn4SingleCardDamage = Math.Max(Stats.HighestPreTurn4CardDamage, d.Stats.HighestPreTurn4SingleCardDamage);
            return d;
        });
    }
    
    private void RecordPartyAdventureHp() => Party.UpdateAdventureHp(Heroes.Select(h => Math.Min(h.CurrentHp(), h.State.BaseStats.MaxHp())).ToArray());
    private void GrantRewardCredits() => Party.UpdateCreditsBy(RewardCredits + playerState.BonusCredits);
    private void GrantRewardClinicVouchers() => Party.UpdateClinicVouchersBy(RewardClinicVouchers);
    private void GrantRewardCards() => Party.Add(RewardCards);
    private void GrantRewardEquipment() => Party.Add(RewardEquipments);
    private void GrantRewardXp() => Party.AwardXp(RewardXp);
    private void GrantXpProgressToNextLevel(float factor) => Party.Heroes.ForEach(h => h.AddXp((h.Levels.XpRequiredForNextLevel * factor).CeilingInt()));
    private int GrantXp()
    {
        var totalXpBefore = Party.Heroes.Sum(h => h.Levels.Xp);
        if (adventureProgress.AdventureProgress.UsesRewardXp)
            GrantRewardXp();
        if (adventureProgress.AdventureProgress.BonusXpLevelFactor > 0)
            GrantXpProgressToNextLevel(adventureProgress.AdventureProgress.BonusXpLevelFactor);
        var totalXpAfter = Party.Heroes.Sum(h => h.Levels.Xp);
        return totalXpAfter - totalXpBefore;
    }

    // Queries
    public bool PlayerWins() =>  EnemyMembers.All(m => m.State.IsUnconscious);
    public bool PlayerLoses() => Heroes.All(m => m.State.IsUnconscious);
    public bool BattleIsOver() => PlayerWins() || PlayerLoses();

    public bool IsMissingOrUnconscious(int memberId) => !Members.TryGetValue(memberId, out var m) || m.IsUnconscious();
    public bool IsHero(int memberId) => _heroesById.ContainsKey(memberId);
    public bool IsEnemy(int memberId) => _enemiesById.ContainsKey(memberId);
    public BaseHero GetHeroById(int memberId) => _heroesById[memberId].Character;
    public Dictionary<int, Color> OwnerTints => _heroesById.ToDictionary(x => x.Key, x => x.Value.Character.Tint);
    public Dictionary<int, Sprite> OwnerBusts => _heroesById.ToDictionary(x => x.Key, x => x.Value.Character.Bust);

    public EnemyInstance GetEnemyById(int memberId) => _enemiesById[memberId];
    public Maybe<Transform> GetMaybeTransform(int memberId) => _uiTransformsById.ValueOrMaybe(memberId);
    
    public AiPreferences GetAiPreferences(int memberId) => _enemiesById.ValueOrMaybe(memberId).Select(e => e.AIPreferences.WithDefaultsBasedOnRole(e.Role), () => new AiPreferences());

    public MemberMaterialType GetMaterialType(int memberId) => _membersById.ValueOrMaybe(memberId).Select(m => m.MaterialType, MemberMaterialType.Unknown);
    
    public Maybe<Transform> GetMaybeCenterPoint(int memberId)
    {
        try
        {
            var maybeTransform = GetMaybeTransform(memberId);
            return maybeTransform.Map(t =>
            {
                var centerPoint = maybeTransform.Value.GetComponentInChildren<CenterPoint>();
                if (centerPoint != null)
                    return centerPoint.transform;

                Log.Warn($"Center Point Missing For: {_membersById[memberId].NameTerm.ToEnglish()}");
                return maybeTransform.Value;
            });
        }
        catch (Exception)
        {
            Log.Warn($"Center Point Missing For: {_membersById[memberId].NameTerm.ToEnglish()}");
            return Maybe<Transform>.Missing();
        }
    }

    public Vector3 GetCenterPoint(TeamType team)
    {
        var target = team == TeamType.Party ? new Multiple(Heroes) : new Multiple(EnemyMembers);
        return GetCenterPoint(target);
    }
    
    public Vector3 GetCenterPoint(Target target)
    {
        if (target.Members.Length == 0)
            return Vector3.zero;
        var bounds = new Bounds(GetMaybeCenterPoint(target.Members[0].Id).Map(cp => cp.position).OrDefault(Vector3.zero), Vector3.zero);
        for (var i = 1; i < target.Members.Length; i++)
            bounds.Encapsulate(GetMaybeCenterPoint(target.Members[i].Id).Map(cp => cp.position).OrDefault(Vector3.zero)); 
        return bounds.center;
    }
    public Member GetMemberByHero(BaseHero hero) => _membersById[_heroesById.First(x => x.Value.Character == hero).Key];
    public Maybe<Member> GetMaybeMemberByHeroCharacterId(int heroCharacterId) => _heroesById
        .FirstAsMaybe(x => x.Value.Character.Id == heroCharacterId)
        .Chain(x => _membersById.ValueOrMaybe(x.Key));
    public Member GetMemberByEnemyIndex(int enemyIndex) => _membersById.VerboseGetValue(enemyIndex + EnemyStartingIndex, nameof(_membersById));
    public int GetEnemyIndexByMemberId(int memberId) => memberId - EnemyStartingIndex;
    public BattleStateSnapshot GetSnapshot() => 
        new BattleStateSnapshot(TurnNumber, Phase, _playedCardHistory.Select(x => x.ToArray()).ToList(), NumberOfCardPlaysRemainingThisTurn, 
            _membersById.ToDictionary(m => m.Key, m => m.Value.GetSnapshot()));

    private void UpdateState(Action update)
    {
        var before = GetSnapshot();
        update();
        Message.Publish(new BattleStateChanged(before, this));
    }
}
