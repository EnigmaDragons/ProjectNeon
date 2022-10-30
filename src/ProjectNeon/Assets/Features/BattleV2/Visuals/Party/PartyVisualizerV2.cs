using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class PartyVisualizerV2 : OnMessage<CharacterAnimationRequested, HighlightCardOwner, UnhighlightCardOwner, DisplaySpriteEffect, ShowHeroBattleThought>
{
    [SerializeField] private BattleState state;
    [SerializeField] private GameObject hero1;
    [SerializeField] private GameObject hero2;
    [SerializeField] private GameObject hero3;
    // TODO: Pull this material management out into separate class
    [SerializeField] private Material defaultSpriteMaterial;
    [SerializeField] private Material cardOwnerMaterial;
    [SerializeField] private CurrentAnimationContext animationContext;
    [SerializeField] private Material evadeMaterial;
    private readonly List<GameObject> _heroes = new List<GameObject>();

    private readonly Dictionary<HeroCharacter, Animator> _animators = new Dictionary<HeroCharacter, Animator>();
    private readonly Dictionary<HeroCharacter, SpriteRenderer> _renderers = new Dictionary<HeroCharacter, SpriteRenderer>();
    private readonly Dictionary<HeroCharacter, HoverCharacter> _hovers  = new Dictionary<HeroCharacter, HoverCharacter>();
    private readonly Dictionary<HeroCharacter, ShieldVisual> _shields  = new Dictionary<HeroCharacter, ShieldVisual>();
    private readonly Dictionary<HeroCharacter, CharacterCreatorStealthTransparency> _stealths = new Dictionary<HeroCharacter, CharacterCreatorStealthTransparency>();
    private readonly Dictionary<HeroCharacter, MemberHighlighter> _highlighters  = new Dictionary<HeroCharacter, MemberHighlighter>();
    private readonly Dictionary<HeroCharacter, TauntEffect> _tauntEffects  = new Dictionary<HeroCharacter, TauntEffect>();
    private readonly Dictionary<HeroCharacter, DamageNumbersController> _damagesNew  = new Dictionary<HeroCharacter, DamageNumbersController>();
    private readonly Dictionary<HeroCharacter, CharacterWordsController> _words  = new Dictionary<HeroCharacter, CharacterWordsController>();
    private readonly Dictionary<HeroCharacter, CenterPoint> _centers = new Dictionary<HeroCharacter, CenterPoint>();
    private readonly Dictionary<HeroCharacter, I2ProgressiveTextRevealWorld> _speech = new Dictionary<HeroCharacter, I2ProgressiveTextRevealWorld>();
    
    public IEnumerator Setup()
    {
        _heroes.ForEach(Destroy);
        _heroes.Clear();
        
        var heroes = state.Party.BaseHeroes;
        if (heroes.Length > 0)
            SetupHero(hero1, heroes[0], 9);
        if (heroes.Length > 1)
            SetupHero(hero2, heroes[1], 5);
        if (heroes.Length > 2)
            SetupHero(hero3, heroes[2], 1);
        var centerPoints = Enumerable.Range(0, 3).Select(x => _heroes.Count > x && _centers.ContainsKey(heroes[x]) ? _centers[heroes[x]].transform.position : Vector3.zero);
        state.PartyArea.WithUiPositions(new[] { hero1.transform, hero2.transform, hero3.transform }, centerPoints);
        yield break;
    }

    public void AfterBattleStateInitialized()
    {
        _damagesNew.ForEach(x => x.Value.Init(state.GetMemberByHero(x.Key)));
        _words.ForEach(x => x.Value.Init(state.GetMemberByHero(x.Key)));
        _hovers.ForEach(x => x.Value.Init(state.GetMemberByHero(x.Key)));
        _shields.ForEach(x => x.Value.Init(state.GetMemberByHero(x.Key)));
        _stealths.ForEach(x => x.Value.Init(state.GetMemberByHero(x.Key)));
        _highlighters.ForEach(x => x.Value.Init(state.GetMemberByHero(x.Key)));
        _tauntEffects.ForEach(x => x.Value.Init(state.GetMemberByHero(x.Key)));
    }

    private void SetupHero(GameObject heroOrigin, HeroCharacter hero, int visualOrder)
    {
        var character = Instantiate(hero.Body, heroOrigin.transform);
        _heroes.Add(character);
        _animators[hero] = character.GetComponentInChildren<Animator>();
        _renderers[hero] = character.GetComponentInChildren<SpriteRenderer>();
        _renderers[hero].material = defaultSpriteMaterial;
         
        var damageEffectController = character.GetComponentInChildren<DamageNumbersController>();
        if (damageEffectController == null)
            Debug.LogError($"{hero.Name} is missing a DamageNumbersController");
        else
            _damagesNew[hero] = damageEffectController;
        
        var wordsController = character.GetComponentInChildren<CharacterWordsController>();
        if (wordsController == null)
            Debug.LogError($"{hero.Name} is missing a {nameof(CharacterWordsController)}");
        else
            _words[hero] = wordsController;

        _hovers[hero] = character.GetCharacterMouseHover(hero.Name);
         
        var centerPoint = character.GetComponentInChildren<CenterPoint>();
        if (centerPoint == null)
            Debug.LogError($"{hero.Name} is missing a CenterPoint");
        else
            _centers[hero] = centerPoint;

        var shield = character.GetComponentInChildren<ShieldVisual>();
        if (shield == null)
            Debug.LogError($"{hero.Name} is missing a {nameof(ShieldVisual)}");
        else
            _shields[hero] = shield;
        
        var highlighter = character.GetComponentInChildren<MemberHighlighter>();
        if (highlighter == null)
            Debug.LogError($"{hero.Name} is missing a {nameof(MemberHighlighter)}");
        else
            _highlighters[hero] = highlighter;
         
        var stealth = character.GetComponentInChildren<CharacterCreatorStealthTransparency>();
        if (stealth == null)
            Debug.LogWarning($"{hero.Name} is missing a {nameof(CharacterCreatorStealthTransparency)}");
        else
            _stealths[hero] = stealth;

        var speech = character.GetComponentInChildren<I2ProgressiveTextRevealWorld>();
        if (speech == null)
            Debug.LogError($"{hero.Name} is missing a {nameof(I2ProgressiveTextRevealWorld)}");
        else
            _speech[hero] = speech;
        
        character.GetComponentInChildren<SpriteRenderer>().sortingOrder = visualOrder;
        
        var tauntEffect = character.GetComponentInChildren<TauntEffect>();
        if (tauntEffect == null)
            Debug.LogWarning($"{hero.Name} is missing a {nameof(TauntEffect)}");
        else
            _tauntEffects[hero] = tauntEffect;
    }

    protected override void Execute(CharacterAnimationRequested e)
    {
        if (!state.IsHero(e.MemberId)) return;
        animationContext.SetAnimation(e);
        
        var hero = state.GetHeroById(e.MemberId);
        var animator = _animators[hero];
        if (animator == null)
            Debug.LogError($"No Animator found for {state.GetHeroById(e.MemberId).Name}");
        else
            StartCoroutine(animator.PlayAnimationUntilFinished(e.Animation.AnimationName, elapsed =>
            {
                DevLog.Write($"Finished {e.Animation} Animation in {elapsed} seconds.");
                Message.Publish(new Finished<CharacterAnimationRequested>());
            }));
    }

    protected override void Execute(HighlightCardOwner msg)
    {
        if (_renderers.Count < 2)
            return;
        
        _renderers.ForEach(kv =>
        {
            kv.Value.material = msg.Member.Name == kv.Key.Name
                ? cardOwnerMaterial
                : defaultSpriteMaterial;
        });
    }

    protected override void Execute(UnhighlightCardOwner msg) => RevertMaterial(msg.Member.Name);

    protected override void Execute(DisplaySpriteEffect msg)
    {
        if (msg.EffectType == SpriteEffectType.Evade)
        {
            _renderers
                .Where(kv => kv.Key.Name == msg.Target.Name)
                .ForEach(kv => kv.Value.material = evadeMaterial);
            StartCoroutine(ExecuteAfterDelay(() => RevertMaterial(msg.Target.Name), 1.4f));
        }
        else
            Log.Error($"Unknown Sprite Effect Type {msg.EffectType}");
    }

    protected override void Execute(ShowHeroBattleThought e)
    {
        if (!state.IsHero(e.MemberId)) return;
        
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        var hero = state.GetHeroById(e.MemberId);
        var s = _speech[hero];
        s.Display(e.Thought, true, false, () => StartCoroutine(ExecuteAfterDelayRealtime(s.Hide, 6f)));
        s.Proceed(true);
    }

    private void RevertMaterial(string memberName)
    {
        _renderers
            .Where(kv => memberName.Equals(kv.Key.Name))
            .ForEach(kv => kv.Value.material = defaultSpriteMaterial);
    }

    private IEnumerator ExecuteAfterDelay(Action a, float delay)
    {
        yield return new WaitForSeconds(delay);
        a();
    }
    
    private IEnumerator ExecuteAfterDelayRealtime(Action a, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        a();
    }
}
